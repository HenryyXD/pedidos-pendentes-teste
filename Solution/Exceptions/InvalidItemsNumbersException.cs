[Serializable]
class InvalidItemsNumbersException : Exception
{
    public InvalidItemsNumbersException() {  }

    public InvalidItemsNumbersException(string message)
        : base(message)
    {

    }
}