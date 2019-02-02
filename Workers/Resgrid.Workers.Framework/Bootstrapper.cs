﻿using Autofac;
using Resgrid.Providers.AddressVerification;
using Resgrid.Providers.Audio;
using Resgrid.Providers.Bus;
using Resgrid.Providers.Cache;
using Resgrid.Providers.EmailProvider;
using Resgrid.Providers.Firebase;
using Resgrid.Providers.GeoLocationProvider;
using Resgrid.Providers.Marketing;
using Resgrid.Providers.NumberProvider;
using Resgrid.Providers.PdfProvider;
using Resgrid.Repositories.DataRepository;
using Resgrid.Services;

namespace Resgrid.Workers.Framework
{
	public static class Bootstrapper
	{
		private static IContainer _container;

		public static void Initialize()
		{
			if (_container == null)
			{
				var builder = new ContainerBuilder();
				builder.RegisterModule(new NonWebDataModule());
				builder.RegisterModule(new ServicesModule());
				builder.RegisterModule(new ProviderModule());
				builder.RegisterModule(new EmailProviderModule());
				builder.RegisterModule(new WorkerFrameworkModule());
				builder.RegisterModule(new BusModule());
				builder.RegisterModule(new AddressVerificationModule());
				builder.RegisterModule(new NumbersProviderModule());
				builder.RegisterModule(new CacheProviderModule());
				builder.RegisterModule(new MarketingModule());
				builder.RegisterModule(new PdfProviderModule());
				builder.RegisterModule(new AudioProviderModule());
				builder.RegisterModule(new FirebaseProviderModule());

				_container = builder.Build();
			}
		}

		public static IContainer GetKernel()
		{
			if (_container == null)
				Initialize();

			return _container;
		}
	}
}
