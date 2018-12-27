using System;

using Castle.Core.Internal;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;

using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Sammak.Windsor.Domain;
using Sammak.Windsor.Util.Extensions;

namespace Sammak.Windsor.Plumbing
{
    public class PersistenceFacility : AbstractFacility
    {
        protected virtual void ConfigurePersistence(Configuration config)
        {
            SchemaMetadataUpdater.QuoteTableAndColumns(config);
        }

        protected virtual AutoPersistenceModel CreateMappingModel()
        {
            var m = AutoMap.Assembly(typeof(EntityBase).Assembly)
                .Where(IsDomainEntity)
                .OverrideAll(ShouldIgnoreProperty)
                .IgnoreBase<EntityBase>();

            return m;
        }

        protected override void Init()
        {
            var config = BuildDatabaseConfiguration();

            Kernel.Register(
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod(_ => config.BuildSessionFactory()), // default lifestyle is singlton; in this case we have only one SessionFactory per container/application
                Component.For<ISession>()
                    .UsingFactoryMethod(k => k.Resolve<ISessionFactory>().OpenSession())
                    .LifestylePerWebRequest()  // makes each session object a transient so that any pending data will be flushed to the database
                );
        }

        protected virtual bool IsDomainEntity(Type t)
        {
            return typeof(EntityBase).IsAssignableFrom(t);
        }

        protected virtual IPersistenceConfigurer SetupDatabase()
        {
            return MsSqlConfiguration.MsSql2012
                .UseOuterJoin()
                .ConnectionString(x => x.FromConnectionStringWithKey("WindsorDbConnection"))
                .ShowSql();
        }

        private Configuration BuildDatabaseConfiguration()
        {
            return Fluently.Configure()
                .Database(SetupDatabase)
                .Mappings(m => m.AutoMappings.Add(CreateMappingModel()))
                .ExposeConfiguration(ConfigurePersistence)
                .BuildConfiguration();
        }

        private void ShouldIgnoreProperty(IPropertyIgnorer property)
        {
            property.IgnoreProperties(p => p.MemberInfo.HasAttribute<DoNotMapAttribute>());
        }

    }
}

/*  Startup.Orm.cs
 *  
    public partial class Startup
    {
        /// <summary>
        /// Gets or sets the session factory.
        /// </summary>
        /// <value>
        /// The session factory.
        /// </value>
        public static ISessionFactory SessionFactory { get; set; }

        /// <summary>
        /// Configures object-relational mapping.
        /// </summary>
        /// <returns></returns>
        internal static ISessionFactory ConfigureOrm()
        {

            //Logger.Trace("ConfigureOrm: Initializing NHibernate Profiler");
            //NHibernateProfiler.Initialize();
            Logger.Trace("ConfigureOrm: Initializing NHibernate.");

            if (SessionFactory != null) return SessionFactory;

            bool useCache = (new Feature_RedisCache()).FeatureEnabled;
            if (useCache) UseOrmCache();

            var connStr = ConfigurationManager.ConnectionStrings["ExamScoringAnalysis"].ConnectionString;
            Debug.Assert(null != connStr);

            SessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                //.ShowSql()    
                .ConnectionString(connStr))
                    
                .Cache(c => SetupCacheSettingsBuilder(c))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ExamFileData>())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditData>())
                .Mappings(m => m.HbmMappings.AddFromAssemblyOf<IExamFileDataRepository>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                .BuildSessionFactory();

            Logger.Trace("ConfigureOrm: Finished initializing NHibernate.");
            return SessionFactory;
        }

        private static void SetupCacheSettingsBuilder(CacheSettingsBuilder c)
        {
            c = c.UseQueryCache().UseSecondLevelCache();

            bool useCache = (new Feature_RedisCache()).FeatureEnabled;
            if (useCache) c = c.ProviderClass<RedisCacheProvider>();
        }
    }
 
*/
