// <auto-generated />
namespace Resgrid.Repositories.DataRepository.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class MigratingMembershipToIdentity : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(MigratingMembershipToIdentity));
        
        string IMigrationMetadata.Id
        {
            get { return "201610131626363_MigratingMembershipToIdentity"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
