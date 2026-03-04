namespace UnitTests.Services
{
    public class ConfigurationBinderServiceTests
    {
        private class NullKeyConfig : Base
        {
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
            public override string? Key => null;
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
            public string? Value { get; set; }
        }

        // Bind root because Key is empty
        private class EmptyKeyConfig : Base
        {
            public override string Key => "";
            public string? Value { get; set; }
        }

        // Bind section because Key is set
        private class SectionKeyConfig : Base
        {
            public override string Key => "MySection";
            public string? Value { get; set; }
        }
        [Fact]
        public void Bind_ShouldThrow_WhenKeyIsNull()
        {
            var config = new ConfigurationBuilder().Build();
            var ex = Assert.Throws<InvalidOperationException>(() => ConfigurationBinderService.Bind<NullKeyConfig>(config));
            Assert.Equal("The configuration section must have a key.", ex.Message);
        }

        [Fact]
        public void Bind_ShouldBindRoot_WhenKeyIsEmpty()
        {
            var inMemoryConfig = new Dictionary<string, string?> { { "Value", "RootValue" } };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            var result = ConfigurationBinderService.Bind<EmptyKeyConfig>(configuration);

            Assert.Equal("RootValue", result.Value);
        }

        [Fact]
        public void Bind_ShouldBindSection_WhenKeyIsSet()
        {
            var inMemorySettings = new Dictionary<string, string?>
    {
        {"MySection:Value", "HelloWorld"}
    };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var result = ConfigurationBinderService.Bind<SectionKeyConfig>(configuration);

            Assert.Equal("HelloWorld", result.Value);
        }


    }


}

