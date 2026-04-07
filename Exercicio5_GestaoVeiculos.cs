using System;
using System.Collections.Generic;

namespace GestaoVeiculos
{
    // Interface IManutencao
    public interface IManutencao
    {
        void Agendar(DateTime data);
        void RealizarManutencao(string descricao, double custo);
        List<Manutencao> ObterHistorico();
    }

    // Enum para Estado do Veículo
    public enum EstadoVeiculo { DISPONIVEL, RESERVADO, MANUTENCAO, AVARIADO }

    // Classe Manutencao
    public class Manutencao
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public double Custo { get; set; }
        public double Km { get; set; }

        public Manutencao(int id, DateTime data, string descricao, double custo, double km)
        {
            Id = id;
            Data = data;
            Descricao = descricao;
            Custo = custo;
            Km = km;
        }

        public override string ToString() => $"{Data:dd/MM/yyyy}: {Descricao} - Custo: {Custo:C} (Km: {Km})";
    }

    // Classe Abstrata Veiculo
    public abstract class Veiculo : IManutencao
    {
        protected string matricula;
        protected string marca;
        protected string modelo;
        protected int ano;
        protected double km;
        protected EstadoVeiculo estado;
        protected List<Manutencao> historicoManutencao;

        public Veiculo(string matricula, string marca, string modelo, int ano, double km)
        {
            this.matricula = matricula;
            this.marca = marca;
            this.modelo = modelo;
            this.ano = ano;
            this.km = km;
            this.estado = EstadoVeiculo.DISPONIVEL;
            this.historicoManutencao = new List<Manutencao>();
        }

        public virtual double CalcularValorDiaria() => 50.0; // Valor base

        public abstract string GetTipo();

        public void AdicionarKm(double kmAdicionais) => km += kmAdicionais;

        // Implementação da Interface IManutencao
        public void Agendar(DateTime data)
        {
            Console.WriteLine($"Manutenção agendada para o veículo {matricula} em {data:dd/MM/yyyy}.");
            estado = EstadoVeiculo.MANUTENCAO;
        }

        public void RealizarManutencao(string descricao, double custo)
        {
            var m = new Manutencao(historicoManutencao.Count + 1, DateTime.Now, descricao, custo, km);
            historicoManutencao.Add(m);
            estado = EstadoVeiculo.DISPONIVEL;
            Console.WriteLine($"Manutenção realizada: {descricao}");
        }

        public List<Manutencao> ObterHistorico() => historicoManutencao;

        public virtual string MostrarDetalhes()
        {
            return $"[{GetTipo()}] {marca} {modelo} ({matricula}) - Ano: {ano}, Km: {km}, Estado: {estado}";
        }
    }

    // Classes Derivadas: Automovel, Motociclo, Camiao
    public class Automovel : Veiculo
    {
        private int numPortas;
        private string tipoCombustivel;
        private int numPassageiros;

        public Automovel(string matricula, string marca, string modelo, int ano, double km, int portas, string combustivel, int passageiros)
            : base(matricula, marca, modelo, ano, km)
        {
            this.numPortas = portas;
            this.tipoCombustivel = combustivel;
            this.numPassageiros = passageiros;
        }

        public override string GetTipo() => "Automóvel";

        public override double CalcularValorDiaria() => 75.0;
    }

    public class Motociclo : Veiculo
    {
        private int cilindrada;
        private bool temSideCar;

        public Motociclo(string matricula, string marca, string modelo, int ano, double km, int cilindrada, bool sideCar)
            : base(matricula, marca, modelo, ano, km)
        {
            this.cilindrada = cilindrada;
            this.temSideCar = sideCar;
        }

        public override string GetTipo() => "Motociclo";

        public override double CalcularValorDiaria() => 40.0;
    }

    public class Camiao : Veiculo
    {
        private double capacidadeCarga;
        private int numEixos;

        public Camiao(string matricula, string marca, string modelo, int ano, double km, double carga, int eixos)
            : base(matricula, marca, modelo, ano, km)
        {
            this.capacidadeCarga = carga;
            this.numEixos = eixos;
        }

        public override string GetTipo() => "Camião";

        public override double CalcularValorDiaria() => 150.0;
    }

    // Classe Reserva
    public class Reserva
    {
        private int id;
        private DateTime dataInicio;
        private DateTime dataFim;
        private double valorTotal;
        private Veiculo veiculo;

        public Reserva(int id, DateTime inicio, DateTime fim, Veiculo v)
        {
            this.id = id;
            this.dataInicio = inicio;
            this.dataFim = fim;
            this.veiculo = v;
            this.valorTotal = CalcularValorTotal();
        }

        private double CalcularValorTotal()
        {
            int dias = (dataFim - dataInicio).Days;
            if (dias <= 0) dias = 1;
            return dias * veiculo.CalcularValorDiaria();
        }

        public void ExibirResumo()
        {
            Console.WriteLine($"\n--- Reserva #{id} ---");
            Console.WriteLine($"Veículo: {veiculo.MostrarDetalhes()}");
            Console.WriteLine($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}");
            Console.WriteLine($"Valor Total: {valorTotal:C}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Criar Veículos
            Automovel carro = new Automovel("LD-20-30-AF", "Toyota", "Corolla", 2022, 15000, 5, "Gasolina", 5);
            Camiao camiao = new Camiao("LD-99-88-ZZ", "Volvo", "FH16", 2020, 120000, 40.0, 3);

            // Exibir Detalhes
            Console.WriteLine("--- Veículos Disponíveis ---");
            Console.WriteLine(carro.MostrarDetalhes());
            Console.WriteLine(camiao.MostrarDetalhes());

            // Criar Reserva
            Reserva r1 = new Reserva(101, DateTime.Now, DateTime.Now.AddDays(5), carro);
            r1.ExibirResumo();

            // Manutenção
            Console.WriteLine("\n--- Manutenção ---");
            camiao.Agendar(DateTime.Now.AddDays(2));
            camiao.RealizarManutencao("Troca de óleo e filtros", 450.00);

            Console.WriteLine("\n--- Histórico de Manutenção Camião ---");
            foreach (var m in camiao.ObterHistorico()) Console.WriteLine(m);
        }
    }
}
