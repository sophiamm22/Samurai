using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.TypedFactory;
using System.Data.Entity;

using Infrastructure.Data;
using Samurai.WebPresentationModel.Controllers;
using Samurai.SqlDataAccess;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Mapping;
using Samurai.WebPresentationModel.Messaging;
using Samurai.Services;
using Samurai.Domain.Value;
using Samurai.Domain.Value.Excel;
using Samurai.Domain.Model;
using Samurai.Domain.Entities;

using Samurai.WebPresentationModel.Messaging.Fixtures.CommandHandlers;

namespace Samurai.Sandbox
{
  public class SamuraiSandboxWindsorInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      var repositoryType = "SaveTestData";
      var basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\value-samurai\";

      container.AddFacility<TypedFactoryFacility>();

      container.Register(Component
                        .For<IValueOptions>()
                        .ImplementedBy<ValueOptions>());

      container.Register(Component
                        .For<DbContext>()
                        .ImplementedBy<ValueSamuraiContext>());

      container.Register(Component
                        .For<IWebRepositoryProvider>()
                        .ImplementedBy<WebRepositoryProvider>()
                        .DependsOn(new
                        {
                          repositoryType = repositoryType,
                          basePath = basePath
                        }
                        ));

      container.Register(Component
                        .For<IBus>()
                        .ImplementedBy<MessageBus>());

      container.Register(AllTypes
                        .FromAssemblyContaining<SqlPredictionRepository>()
                        .Where(t => t.Name.StartsWith("Sql") &&
                                    t.Name.EndsWith("Repository"))
                        .WithService
                        .AllInterfaces());

      container.Register(AllTypes
                        .FromAssemblyContaining<FootballFixtureService>()
                        .Where(t => t.Name.EndsWith("Service"))
                        .WithService
                        .AllInterfaces());

      container.Register(AllTypes
                        .FromAssemblyContaining<IndexFootballFixturesHandler>()
                        .Where(t => t.Name.EndsWith("Handler"))
                        .WithService
                        .AllInterfaces());

      container.Register(AllTypes
                        .FromAssemblyContaining<PredictionStrategyProvider>()
                        .Where(t => !t.Name.StartsWith("Excel"))
                        .WithService
                        .AllInterfaces());

      container.Register(Component
                        .For<IMessageHandlerFactory>()
                        .AsFactory());

      container.Register(Component
                        .For<ICommandHandlerFactory>()
                        .AsFactory());
    }
  }
}
