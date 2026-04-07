using System;
using System.Collections.Generic;

namespace SistemaBancario
{
    // Interface IAuditavel
    public interface IAuditavel
    {
        void RegistrarAuditoria(string operacao);
        List<string> ObterLog();
    }

    // Enum para Tipo de Transação
    public enum TipoTransacao { DEPOSITO, LEVANTAMENTO, TRANSFERENCIA }

    // Classe Transacao
    public class Transacao
    {
        private int id;
        private DateTime data;
        private double valor;
        private TipoTransacao tipo;
        private string descricao;

        public Transacao(int id, double valor, TipoTransacao tipo, string descricao)
        {
            this.id = id;
            this.data = DateTime.Now;
            this.valor = valor;
            this.tipo = tipo;
            this.descricao = descricao;
        }

        public override string ToString()
        {
            return $"{data:dd/MM/yyyy HH:mm} - {tipo}: {valor:C} ({descricao})";
        }
    }

    // Classe Cliente
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Nif { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }

        public Cliente(int id, string nome, string nif, string telefone, string email)
        {
            Id = id;
            Nome = nome;
            Nif = nif;
            Telefone = telefone;
            Email = email;
        }
    }

    // Classe Abstrata ContaBancaria
    public abstract class ContaBancaria : IAuditavel
    {
        protected string iban;
        protected Cliente titular;
        protected double saldo;
        protected DateTime dataAbertura;
        protected List<Transacao> historico;
        protected List<string> logsAuditoria;

        public ContaBancaria(string iban, Cliente titular, double saldoInicial)
        {
            this.iban = iban;
            this.titular = titular;
            this.saldo = saldoInicial;
            this.dataAbertura = DateTime.Now;
            this.historico = new List<Transacao>();
            this.logsAuditoria = new List<string>();
        }

        public virtual bool Depositar(double valor)
        {
            if (valor <= 0) return false;
            saldo += valor;
            historico.Add(new Transacao(historico.Count + 1, valor, TipoTransacao.DEPOSITO, "Depósito em conta"));
            RegistrarAuditoria($"Depósito de {valor:C}");
            return true;
        }

        public virtual bool Levantar(double valor)
        {
            if (valor <= 0 || valor > saldo) return false;
            saldo -= valor;
            historico.Add(new Transacao(historico.Count + 1, valor, TipoTransacao.LEVANTAMENTO, "Levantamento em conta"));
            RegistrarAuditoria($"Levantamento de {valor:C}");
            return true;
        }

        public double GetSaldo() => saldo;

        // Método Abstrato
        public abstract double CalcularJuros();

        public List<Transacao> GetHistorico() => historico;

        // Implementação da Interface IAuditavel
        public void RegistrarAuditoria(string operacao)
        {
            logsAuditoria.Add($"{DateTime.Now}: {operacao} na conta {iban}");
        }

        public List<string> ObterLog() => logsAuditoria;

        public virtual string MostrarDetalhes()
        {
            return $"IBAN: {iban}, Titular: {titular.Nome}, Saldo: {saldo:C}";
        }
    }

    // Classe ContaOrdem
    public class ContaOrdem : ContaBancaria
    {
        private double limiteDescoberto;

        public ContaOrdem(string iban, Cliente titular, double saldoInicial, double limite)
            : base(iban, titular, saldoInicial)
        {
            this.limiteDescoberto = limite;
        }

        public override bool Levantar(double valor)
        {
            if (valor <= 0 || valor > (saldo + limiteDescoberto)) return false;
            saldo -= valor;
            historico.Add(new Transacao(historico.Count + 1, valor, TipoTransacao.LEVANTAMENTO, "Levantamento com limite descoberto"));
            RegistrarAuditoria($"Levantamento de {valor:C} (Limite Descoberto)");
            return true;
        }

        public override double CalcularJuros()
        {
            // Conta ordem geralmente não rende juros
            return 0.0;
        }

        public double GetLimite() => limiteDescoberto;
    }

    // Classe ContaPoupanca
    public class ContaPoupanca : ContaBancaria
    {
        private double taxaJuroAnual;

        public ContaPoupanca(string iban, Cliente titular, double saldoInicial, double taxa)
            : base(iban, titular, saldoInicial)
        {
            this.taxaJuroAnual = taxa;
        }

        public override double CalcularJuros()
        {
            double juros = saldo * (taxaJuroAnual / 100);
            saldo += juros;
            RegistrarAuditoria($"Juros creditados: {juros:C}");
            return juros;
        }

        public double GetTaxa() => taxaJuroAnual;
    }

    // Classe Transferencia (Herda de Transacao)
    public class Transferencia : Transacao
    {
        private ContaBancaria contaOrigem;
        private ContaBancaria contaDestino;

        public Transferencia(int id, double valor, string descricao, ContaBancaria origem, ContaBancaria destino)
            : base(id, valor, TipoTransacao.TRANSFERENCIA, descricao)
        {
            this.contaOrigem = origem;
            this.contaDestino = destino;
        }

        public static bool Executar(ContaBancaria origem, ContaBancaria destino, double valor)
        {
            if (origem.Levantar(valor))
            {
                destino.Depositar(valor);
                return true;
            }
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Criar Cliente
            Cliente cliente1 = new Cliente(1, "Maria Luísa", "123456789", "923-111-222", "maria@email.com");

            // Criar Contas
            ContaOrdem cOrdem = new ContaOrdem("AO06.0001.0000.1234.5678.90", cliente1, 500.00, 200.00);
            ContaPoupanca cPoupanca = new ContaPoupanca("AO06.0001.0000.9876.5432.10", cliente1, 2000.00, 5.0);

            // Operações
            Console.WriteLine("--- Estado Inicial ---");
            Console.WriteLine(cOrdem.MostrarDetalhes());
            Console.WriteLine(cPoupanca.MostrarDetalhes());

            Console.WriteLine("\n--- Executando Transferência de 300€ ---");
            if (Transferencia.Executar(cOrdem, cPoupanca, 300.00))
                Console.WriteLine("Transferência concluída com sucesso!");

            Console.WriteLine("\n--- Calculando Juros Poupança ---");
            double juros = cPoupanca.CalcularJuros();
            Console.WriteLine($"Juros creditados: {juros:C}");

            Console.WriteLine("\n--- Estado Final ---");
            Console.WriteLine(cOrdem.MostrarDetalhes());
            Console.WriteLine(cPoupanca.MostrarDetalhes());

            Console.WriteLine("\n--- Logs de Auditoria (Conta Ordem) ---");
            foreach (var log in cOrdem.ObterLog()) Console.WriteLine(log);
        }
    }
}
