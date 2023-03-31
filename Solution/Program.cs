using Newtonsoft.Json;
using Solution.Model;
using System.Text;

namespace Solution
{
  class Program
  {
    static void Main(string[] args)
    {
      string pedidosPath = @"../Pedidos";
      string notasPath = @"../Notas";
      string outputSolutionPath = @"../pedidos_pedentes.txt";

      List<Pedido> pedidos = GetObjectList<Pedido, Item>(pedidosPath, (id, items) => new Pedido(id, items));
      List<Nota> notas = GetObjectList<Nota, ItemNota>(notasPath, (id, itemNotas) => new Nota(id, itemNotas));
      List<Pedido> pedidosPendentes = GetPedidosPendentes(pedidos, notas);

      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Pedidos Pendentes:\n");
      pedidosPendentes.ForEach(pendente =>
      {
        sb.AppendLine($"Pedido {pendente.Id}");
        decimal? tot = pedidos.Find(p => p.Id == pendente.Id)?.getValorTotal();
        sb.AppendLine($"Valor total do pedido: R${tot}");
        sb.AppendLine($"Saldo do valor: R${pendente.getValorTotal()}");
        foreach (Item i in pendente.Items)
        {
          sb.AppendLine(i.getItemAndQtdToString());
        }
        sb.Append("\n");
      });

      using (StreamWriter sw = new StreamWriter(outputSolutionPath))
      {
        sw.Write(sb);
      }
    }

    static List<T> DeserializeToList<T>(string filePath)
    {
      List<T> objectsList = new List<T>();
      string[] lines = File.ReadAllLines(filePath);
      foreach (string line in lines)
      {
        T? obj = JsonConvert.DeserializeObject<T>(line, new DecimalFormatConverter());
        if (obj != null) objectsList.Add(obj);
      }

      return objectsList;
    }

    static List<T> GetObjectList<T, I>(string folderPath, Func<string, List<I>, T> constructorFunc)
    {
      List<string> filenames = Directory.GetFiles(folderPath, "*.txt").ToList();
      List<T> list = new List<T>();
      foreach (string fileFullPath in filenames)
      {
        List<I> items = DeserializeToList<I>(fileFullPath);
        //ex.: "P1"[1] = "1"
        string id = Path.GetFileNameWithoutExtension(fileFullPath)[1].ToString();
        list.Add(constructorFunc(id, items));
      }
      return list;
    }

    static List<Pedido> GetPedidosPendentes(List<Pedido> pedidos, List<Nota> notas)
    {
      var pedidosPendentesCopy = JsonConvert.DeserializeObject<List<Pedido>>(JsonConvert.SerializeObject(pedidos))!;

      var somaDaQuantidadeDeProduto = new Dictionary<(string IdNota, string IdPedido, uint NumeroItem), int>();

      somaQtdProdutoPorNota(notas, ref somaDaQuantidadeDeProduto);

      atualizaPedidosPendentes(ref pedidosPendentesCopy, somaDaQuantidadeDeProduto);

      return pedidosPendentesCopy;
    }

    private static void somaQtdProdutoPorNota(List<Nota> notas, ref Dictionary<(string IdNota, string IdPedido, uint NumeroItem), int> somaDaQuantidadeDeProduto)
    {
      foreach (Nota n in notas)
      {
        foreach (ItemNota iNota in n.ItemNota)
        {
          var key = (n.Id, iNota.IdPedido, iNota.NumeroItem);
          if (!somaDaQuantidadeDeProduto.ContainsKey(key))
          {
            somaDaQuantidadeDeProduto.Add(key, 0);
          }
          somaDaQuantidadeDeProduto[key] += iNota.QtdProduto;
        }
      }
    }

    private static void atualizaPedidosPendentes(ref List<Pedido> pedidosPendentes, Dictionary<
    (string IdNota,
      string IdPedido,
      uint NumeroItem),
      int> somaDaQuantidadeDeProduto)
    {
      foreach (var somaProd in somaDaQuantidadeDeProduto)
      {
        (int pedidoIndex, int itemIndex) = findIndexPedidoAndItemByItemNota(
          ref pedidosPendentes,
          somaProd.Key.IdPedido,
          somaProd.Key.NumeroItem
        );

        validateIndexPedidoAndItem(pedidoIndex, itemIndex, somaProd);

        atualizaQtdProdutosPedido(ref pedidosPendentes, pedidoIndex, itemIndex, somaProd.Value);
      }
    }

    static (int, int) findIndexPedidoAndItemByItemNota(ref List<Pedido> pedidos, string IdPedido, uint NumeroItem)
    {
      int pedidoIndex = pedidos.FindIndex(p => p.Id == IdPedido);
      if (pedidoIndex == -1) return (-1, 0);
      int itemIndex = pedidos[pedidoIndex].Items.FindIndex(i => i.NumeroItem == NumeroItem);
      if (itemIndex == -1) return (0, -1);

      return (pedidoIndex, itemIndex);
    }

    private static void validateIndexPedidoAndItem(int pedidoIndex, int itemIndex, KeyValuePair<(string IdNota, string IdPedido, uint NumeroItem), int> somaProd)
    {
      if (pedidoIndex == -1 || itemIndex == -1)
      {
        string message = "";
        if (pedidoIndex == -1)
        {
          message = $"Não foi encotrado o pedido com id \"{somaProd.Key.IdPedido}\" " +
          $"dado pela nota \"{somaProd.Key.IdNota}\"";
        }
        else
        {
          message = $"Não foi encotrado o número_item \"{somaProd.Key.NumeroItem}\" " +
                    $"do pedido \"{somaProd.Key.IdPedido}\" dado pela nota \"{somaProd.Key.IdNota}\"";
        }
        throw new InvalidIdException(message);
      }
    }

    private static void atualizaQtdProdutosPedido(
      ref List<Pedido> pedidosPendentes,
      int pedidoIndex,
      int itemIndex,
      int QtdProduto
    )
    {
      int qtdProdutoAtual = pedidosPendentes[pedidoIndex].Items[itemIndex].QtdProduto;

      int novaQtd = qtdProdutoAtual - QtdProduto;

      if (novaQtd < 0)
      {
        throw new InvalidProductQuantityException(
          "A soma da quantidade de produto nas notas é maior que a " +
          $"quantidade de produto do pedido \"{pedidosPendentes[pedidoIndex].Id}\"" +
          $" item \"{pedidosPendentes[pedidoIndex].Items[itemIndex].NumeroItem}\"" +
          $"\nQuantidade total das notas: {QtdProduto}" +
          $"\nQuantidade de produtos do pedido: {qtdProdutoAtual}");
      }
      else if (novaQtd == 0)
      {
        pedidosPendentes[pedidoIndex].Items.RemoveAt(itemIndex);
        if (pedidosPendentes[pedidoIndex].Items.Count == 0)
        {
          pedidosPendentes.RemoveAt(pedidoIndex);
        }
      }
      else
      {
        pedidosPendentes[pedidoIndex].Items[itemIndex].QtdProduto = novaQtd;
      }
    }
  }
}