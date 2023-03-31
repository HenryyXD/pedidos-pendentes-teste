using Newtonsoft.Json;

namespace Solution.Model
{
  class ItemNota
  {
    [JsonProperty("id_pedido")]
    public string IdPedido { get; private set; }
    [JsonProperty("número_item")]
    public uint NumeroItem { get; private set; }
    [JsonProperty("quantidade_produto")]
    public int QtdProduto { get; private set; }

    public ItemNota(string idPedido, uint numeroItem, int qtdProduto)
    {
      this.IdPedido = idPedido;
      this.NumeroItem = numeroItem;
      this.QtdProduto = qtdProduto;
    }

    public override string ToString()
    {
      return $"{{ IdPedido: {IdPedido}, NumeroItem: {NumeroItem}, QtdProduto: {QtdProduto} }}";
    }
  }
}