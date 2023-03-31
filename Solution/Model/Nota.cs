namespace Solution.Model
{
  class Nota
  {
    public string Id { get; private set; }
    public List<ItemNota> ItemNota { get; private set; }

    public Nota(string id, List<ItemNota> itemNota)
    {
      Id = id;
      ItemNota = itemNota;
    }

    public override string ToString()
    {
      return $"Id: {Id} \nItemNota: \n{String.Join<ItemNota>("\n", ItemNota.ToArray())} \n ";
    }
  }
}