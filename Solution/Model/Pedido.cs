namespace Solution.Model
{
  class Pedido
  {
    public string Id { get; private set; }
    public List<Item> Items { get; private set; }

    public Pedido(string id, List<Item> items)
    {
      Id = id;
      Items = items;
      ValidateListOfItems();
    }

    private void ValidateListOfItems()
    {
      if (ItemsEmptyOrNull()) return;

      Item? duplicatedItem = CheckDuplicatedItems();

      if (duplicatedItem != null)
      {
        throw new DuplicateItemException(string.Format(
          "Item duplicado: \"{0}\" no pedido {1}", duplicatedItem, Id
        ));
      }

      uint maxNumeroItem = Items.Max(i => i.NumeroItem);
      uint minNumeroItem = Items.Min(i => i.NumeroItem);

      if (minNumeroItem != 1)
      {
        throw new InvalidItemsNumbersException(string.Format(
          "O menor número_item do pedido {0} deve ser 1", Id
        ));
      }

      if (!isNumbersInSequence(minNumeroItem, maxNumeroItem, Items.Count))
      {
        throw new InvalidItemsNumbersException(String.Format(
          "O número_item do pedido {0} deve haver todos os números consecutivos de 1 ao maior número de items", 
          Id
        ));
      }
    }

    private bool ItemsEmptyOrNull()
    {
      return Items == null || Items.Count == 0;
    }

    private Item? CheckDuplicatedItems()
    {
      var firstDuplicateItem = Items
          .GroupBy(i => i.NumeroItem)
          .FirstOrDefault(i => i.Skip(1).Any());

      return firstDuplicateItem?.First();
    }

    private bool isNumbersInSequence(uint min, uint max, int sequenceLength)
    {
      return max - min + 1 == sequenceLength;
    }

    public decimal getValorTotal() {
      return Items.Sum(i => i.QtdProduto * i.ValorUnitarioProduto);
    }

    public override string ToString()
    {
      return $"Id: {Id} \nItems: \n{String.Join<Item>("\n", Items.ToArray())} \n ";
    }

  }
}