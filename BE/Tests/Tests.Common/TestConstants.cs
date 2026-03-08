namespace Tests.Common;

public static class TestConstants
{
    public const string TestsConnectionString =
        "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres-Wkd@saf2kf!Gf2%;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=20;Include Error Detail=true";

    public static class TestCollections
    {
        public const string IntegrationTests = "IntegrationTests";
        public const string FrontOfficeUnitTests = "FrontOfficeUnitTests";
    }

    public static class BaseUri
    {
        public static readonly Uri FrontOfficeTestHostUri = new("http://localhost:2020");
        public static readonly Uri IdentityTestHostUri = new("http://localhost:2021");
        public static readonly Uri MatchmakerTestHostUri = new("http://localhost:2022");
        public static readonly Uri ChronicleTestHostUri = new("http://localhost:2023");
    }
}