namespace m1w2s3.httpClient.flurl.Transacoes;

public sealed class Payer
{
    public Payer(Guid? id, string name, string document, string email, string phoneNumber)
    {
        Id = id;
        Name = name;
        Document = document;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public Guid? Id { get; }
    public string Name { get; }
    public string Document { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
}
