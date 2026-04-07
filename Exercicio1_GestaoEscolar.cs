using System;
using System.Collections.Generic;

namespace GestaoEscolar
{
    // Classe Base Pessoa
    public class Pessoa
    {
        protected string nome;
        protected DateTime dataNascimento;
        protected string telefone;

        public Pessoa(string nome, DateTime dataNascimento, string telefone)
        {
            this.nome = nome;
            this.dataNascimento = dataNascimento;
            this.telefone = telefone;
        }

        public string GetNome() => nome;

        public int GetIdade()
        {
            int idade = DateTime.Now.Year - dataNascimento.Year;
            if (DateTime.Now < dataNascimento.AddYears(idade)) idade--;
            return idade;
        }

        public virtual string MostrarInformacoes()
        {
            return $"Nome: {nome}, Idade: {GetIdade()}, Telefone: {telefone}";
        }
    }

    // Classe Aluno herda de Pessoa
    public class Aluno : Pessoa
    {
        private int numero;
        private string curso;
        private List<double> notas;

        public Aluno(string nome, DateTime dataNascimento, string telefone, int numero, string curso)
            : base(nome, dataNascimento, telefone)
        {
            this.numero = numero;
            this.curso = curso;
            this.notas = new List<double>();
        }

        public void AdicionarNota(double n) => notas.Add(n);

        public double CalcularMedia()
        {
            if (notas.Count == 0) return 0;
            double soma = 0;
            foreach (var nota in notas) soma += nota;
            return soma / notas.Count;
        }

        public string ObterSituacao()
        {
            double media = CalcularMedia();
            return media >= 10 ? "Aprovado" : "Reprovado";
        }

        public override string MostrarInformacoes()
        {
            return base.MostrarInformacoes() + $", Nº: {numero}, Curso: {curso}, Média: {CalcularMedia():F2}, Situação: {ObterSituacao()}";
        }
    }

    // Classe Professor herda de Pessoa
    public class Professor : Pessoa
    {
        private string especialidade;
        private string departamento;
        private double salario;

        public Professor(string nome, DateTime dataNascimento, string telefone, string especialidade, string departamento, double salario)
            : base(nome, dataNascimento, telefone)
        {
            this.especialidade = especialidade;
            this.departamento = departamento;
            this.salario = salario;
        }

        public void Leccionar() => Console.WriteLine($"O professor {nome} está leccionando {especialidade}.");

        public double AvaliarAluno(Aluno a)
        {
            double media = a.CalcularMedia();
            Console.WriteLine($"O professor {nome} avaliou o aluno {a.GetNome()}. Média: {media:F2}");
            return media;
        }

        public double GetSalario() => salario;

        public override string MostrarInformacoes()
        {
            return base.MostrarInformacoes() + $", Especialidade: {especialidade}, Departamento: {departamento}, Salário: {salario:C}";
        }
    }

    // Classe Turma (Agregação/Composição)
    public class Turma
    {
        private string codigo;
        private int ano;
        private int capacidade;
        private Professor professorResponsavel;
        private List<Aluno> alunos;

        public Turma(string codigo, int ano, int capacidade, Professor professor)
        {
            this.codigo = codigo;
            this.ano = ano;
            this.capacidade = capacidade;
            this.professorResponsavel = professor;
            this.alunos = new List<Aluno>();
        }

        public bool AdicionarAluno(Aluno a)
        {
            if (alunos.Count < capacidade)
            {
                alunos.Add(a);
                return true;
            }
            Console.WriteLine("Turma cheia!");
            return false;
        }

        public void ListarAlunos()
        {
            Console.WriteLine($"\n--- Alunos da Turma {codigo} (Prof. {professorResponsavel.GetNome()}) ---");
            foreach (var aluno in alunos)
            {
                Console.WriteLine(aluno.MostrarInformacoes());
            }
        }
    }

    // Programa Principal para Teste
    class Program
    {
        static void Main(string[] args)
        {
            // Criar Professor
            Professor prof = new Professor("Joaquim Ventura", new DateTime(1980, 5, 15), "923-000-000", "Engenharia de Software", "Informática", 2500.00);

            // Criar Alunos
            Aluno aluno1 = new Aluno("Ana Silva", new DateTime(2000, 10, 20), "912-111-222", 101, "Engenharia Informática");
            aluno1.AdicionarNota(14.5);
            aluno1.AdicionarNota(16.0);

            Aluno aluno2 = new Aluno("Pedro Santos", new DateTime(1999, 3, 12), "915-333-444", 102, "Engenharia Informática");
            aluno2.AdicionarNota(8.0);
            aluno2.AdicionarNota(9.5);

            // Criar Turma e adicionar alunos
            Turma turmaA = new Turma("EI-2026", 2026, 30, prof);
            turmaA.AdicionarAluno(aluno1);
            turmaA.AdicionarAluno(aluno2);

            // Executar ações
            prof.Leccionar();
            prof.AvaliarAluno(aluno1);
            turmaA.ListarAlunos();

            Console.WriteLine("\n--- Detalhes do Professor ---");
            Console.WriteLine(prof.MostrarInformacoes());
        }
    }
}
