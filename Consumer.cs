
public record Consumer(int Id, string Name, int Age, AddressInfo Address)
{
    public Consumer WithConsumerName(string Name)
    {
        return this with { Name = Name };
    }

    public Consumer WithChangeAddress(AddressInfo addressInfo)
    {
        if (addressInfo is null)
        {
            throw new ArgumentNullException(nameof(addressInfo));
        }

        return this with { Address = addressInfo };
    }

    public static Consumer Create(int Id, string Name, int Age, AddressInfo addressInfo)
    {
        if (addressInfo is null)
        {
            throw new ArgumentNullException(nameof(addressInfo));
        }

        return new Consumer(Id, Name, Age, addressInfo);
    }
}


public record AddressInfo(string Address, string Tel, string No)
{
    public static AddressInfo Create(string Address, string Tel, string No)
    {
         return new AddressInfo(Address, Tel, No);
    }
}
