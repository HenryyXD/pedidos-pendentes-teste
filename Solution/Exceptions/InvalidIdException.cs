[Serializable]
class InvalidIdException : Exception
{
    public InvalidIdException() {  }

    public InvalidIdException(string message)
        : base(message)
    {

    }
}