namespace RecordHub.IdentityService.Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(AccountDbContext ctx)
        {
            ctx.Database.EnsureCreated();

        }
    }
}
