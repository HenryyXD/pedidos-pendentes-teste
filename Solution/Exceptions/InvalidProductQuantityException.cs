[Serializable]
class InvalidProductQuantityException : Exception
{
    public InvalidProductQuantityException() {  }

    public InvalidProductQuantityException(string message)
        : base(message)
    {

    }
}