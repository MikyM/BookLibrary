using Application.Services;
using AttributeBasedRegistration.Autofac;
using Autofac;
using DataExplorer;
using DataExplorer.EfCore;
using DataExplorer.EfCore.Extensions;
using Domain;
using IdGen;
using ServiceLifetime = AttributeBasedRegistration.ServiceLifetime;

namespace API;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assembly  = typeof(InitializationService).Assembly;
        var entityAssembly  = typeof(Book).Assembly;
        var assemblyArray = new[] { assembly };
        
        builder.AddDataExplorer(opt =>
        {
            opt.AutoMapperProfileAssembliesAccessor = () => assemblyArray;
            opt.AutoMapperConfiguration = mapper =>
            {
                mapper.AllowNullDestinationValues = true;
                mapper.AllowNullCollections = true;
            };

            opt.AddSnowflakeIdGeneration(1, () =>
            {
                const SequenceOverflowStrategy sequenceOverflowStrategy = SequenceOverflowStrategy.SpinWait;
                var structure = new IdStructure(45, 2, 16);
                var defaultTimeSource = new DefaultTimeSource(new DateTime(2024, 12, 15, 0, 0, 0, DateTimeKind.Utc));
                
                return new IdGeneratorOptions(structure, defaultTimeSource, sequenceOverflowStrategy);
            });
            
            opt.AddEfCore(efOpt =>
            {
                efOpt.DateTimeStrategy = DateTimeStrategy.UtcNow;
                efOpt.EnableIncludeCache = true;
                efOpt.DataServiceLifetime = ServiceLifetime.InstancePerLifetimeScope;
                efOpt.BaseGenericDataServiceLifetime = ServiceLifetime.InstancePerLifetimeScope;
            }, assembly, entityAssembly);
        });
        
        builder.RegisterAsyncInterceptorAdapter();
        
        base.Load(builder);
    }
}