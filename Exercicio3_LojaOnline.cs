using System;
using System.Collections.Generic;

namespace LojaOnline
{
    // Classe Abstrata Produto
    public abstract class Produto
    {
        private int id;
        private string nome;
        private string descricao;
        protected double preco;
        private int stock;

        public Produto(int id, string nome, string descricao, double preco, int stock)
        {
            this.id = id;
            this.nome = nome;
            this.descricao = descricao;
            this.preco = preco;
            this.stock = stock;
        }

        public double GetPreco() => preco;

        public bool TemStock() => stock > 0;

        public void ReduzirStock(int qtd)
        {
            if (stock >= qtd) stock -= qtd;
            else Console.WriteLine("Stock insuficiente!");
        }

        // Método Abstrato
        public abstract double CalcularFrete();

        public virtual string MostrarDetalhes()
        {
            return $"ID: {id}, Nome: {nome}, Preço: {preco:C}, Stock: {stock}";
        }
    }

    // Classe ProdutoFisico herda de Produto
    public class ProdutoFisico : Produto
    {
        private double peso;
        private string dimensoes;

        public ProdutoFisico(int id, string nome, string descricao, double preco, int stock, double peso, string dimensoes)
            : base(id, nome, descricao, preco, stock)
        {
            this.peso = peso;
            this.dimensoes = dimensoes;
        }

        public override double CalcularFrete()
        {
            // Regra simples: 5€ por kg
            return peso * 5.0;
        }

        public double GetPeso() => peso;

        public override string MostrarDetalhes()
        {
            return base.MostrarDetalhes() + $", Peso: {peso}kg, Dimensões: {dimensoes}, Frete: {CalcularFrete():C}";
        }
    }

    // Classe ProdutoDigital herda de Produto
    public class ProdutoDigital : Produto
    {
        private double tamanhoMB;
        private string formatoFicheiro;

        public ProdutoDigital(int id, string nome, string descricao, double preco, int stock, double tamanhoMB, string formatoFicheiro)
            : base(id, nome, descricao, preco, stock)
        {
            this.tamanhoMB = tamanhoMB;
            this.formatoFicheiro = formatoFicheiro;
        }

        public override double CalcularFrete()
        {
            // Produtos digitais não têm frete
            return 0.0;
        }

        public string GerarLink() => $"https://loja.com/download/{Guid.NewGuid()}";

        public override string MostrarDetalhes()
        {
            return base.MostrarDetalhes() + $", Tamanho: {tamanhoMB}MB, Formato: {formatoFicheiro}, Link: {GerarLink()}";
        }
    }

    // Classe Categoria
    public class Categoria
    {
        private int id;
        private string nome;
        private string descricao;
        private List<Produto> produtos;

        public Categoria(int id, string nome, string descricao)
        {
            this.id = id;
            this.nome = nome;
            this.descricao = descricao;
            this.produtos = new List<Produto>();
        }

        public void AdicionarProduto(Produto p) => produtos.Add(p);

        public void ListarProdutos()
        {
            Console.WriteLine($"\n--- Categoria: {nome} ---");
            foreach (var p in produtos) Console.WriteLine(p.MostrarDetalhes());
        }
    }

    // Classe Cliente
    public class Cliente
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Morada { get; set; }

        public Cliente(string nome, string email, string morada)
        {
            Nome = nome;
            Email = email;
            Morada = morada;
        }
    }

    // Classe Carrinho e ItemCarrinho (Composição)
    public class ItemCarrinho
    {
        public Produto Produto { get; private set; }
        public int Quantidade { get; private set; }

        public ItemCarrinho(Produto produto, int quantidade)
        {
            Produto = produto;
            Quantidade = quantidade;
        }

        public double CalcularSubtotal() => Produto.GetPreco() * Quantidade;
    }

    public class Carrinho
    {
        private int id;
        private DateTime dataCriacao;
        private List<ItemCarrinho> itens;
        private Cliente cliente;

        public Carrinho(int id, Cliente cliente)
        {
            this.id = id;
            this.dataCriacao = DateTime.Now;
            this.cliente = cliente;
            this.itens = new List<ItemCarrinho>();
        }

        public void AdicionarItem(Produto p, int qtd)
        {
            if (p.TemStock())
            {
                itens.Add(new ItemCarrinho(p, qtd));
                p.ReduzirStock(qtd);
            }
        }

        public double CalcularTotal()
        {
            double total = 0;
            foreach (var item in itens)
            {
                total += item.CalcularSubtotal() + item.Produto.CalcularFrete();
            }
            return total;
        }

        public void ExibirResumo()
        {
            Console.WriteLine($"\n--- Resumo do Carrinho ({cliente.Nome}) ---");
            foreach (var item in itens)
            {
                Console.WriteLine($"- {item.Produto.MostrarDetalhes()} x{item.Quantidade}");
            }
            Console.WriteLine($"TOTAL (com frete): {CalcularTotal():C}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Criar Categoria e Produtos
            Categoria informatica = new Categoria(1, "Informática", "Equipamentos tecnológicos");
            ProdutoFisico monitor = new ProdutoFisico(10, "Monitor 24\"", "Monitor LED Full HD", 150.00, 10, 3.5, "50x30x10");
            ProdutoDigital ebook = new ProdutoDigital(20, "E-book C#", "Guia completo de POO", 29.90, 999, 5.2, "PDF");

            informatica.AdicionarProduto(monitor);
            informatica.AdicionarProduto(ebook);
            informatica.ListarProdutos();

            // Criar Cliente e Carrinho
            Cliente cliente = new Cliente("Carlos Mendes", "carlos@email.com", "Rua Principal, 123");
            Carrinho carrinho = new Carrinho(1, cliente);

            carrinho.AdicionarItem(monitor, 1);
            carrinho.AdicionarItem(ebook, 1);

            carrinho.ExibirResumo();
        }
    }
}
