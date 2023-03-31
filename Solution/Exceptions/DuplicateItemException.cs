[Serializable]
class DuplicateItemException : Exception
{
    public DuplicateItemException() {  }

    public DuplicateItemException(string message)
        : base(message)
    {

    }
}