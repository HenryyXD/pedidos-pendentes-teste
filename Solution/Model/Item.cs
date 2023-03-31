using Newtonsoft.Json;

namespace Solution.Model
{
  class Item
  {
    [JsonProperty("número_item")]
    public uint NumeroItem { get; private set; }
    [JsonProperty("código_produto")]
    public string CodProduto { get; private set; }
    [JsonProperty("valor_unitário_produto")]
    public decimal ValorUnitarioProduto { get; private set; }
    [JsonProperty("quantidade_produto")]
    public int QtdProduto { get; set; }

    public Item(uint numeroItem, string codProduto, decimal valorUnitarioProduto, int qtdProduto)
    {
      this.NumeroItem = numeroItem;
      this.CodProduto = codProduto;
      this.ValorUnitarioProduto = valorUnitarioProduto;
      this.QtdProduto = qtdProduto;
    }

    public string getItemAndQtdToString() {
      return String.Format(" - Item {0}: saldo de quantidade = {1}", NumeroItem, QtdProduto);
    }


    public override string ToString()
    {
      return $"{{ NumeroItem: {NumeroItem}, CodProduto: {CodProduto}, ValorUnitarioProduto: {ValorUnitarioProduto}, QtdProduto: {QtdProduto} }}";
    }
  }
}