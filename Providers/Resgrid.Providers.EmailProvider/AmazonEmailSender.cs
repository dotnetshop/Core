﻿using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using MimeKit;
using Resgrid.Model.Providers;
using System;
using System.IO;

namespace Resgrid.Providers.EmailProvider
{
	public class AmazonEmailSender: IAmazonEmailSender
	{
		public void SendDistributionListEmail(MimeMessage message)
		{
			var stream = new MemoryStream();
			message.WriteTo(stream);
			var request = new SendRawEmailRequest() { RawMessage = new RawMessage() { Data = stream } };

			// Choose the AWS region of the Amazon SES endpoint you want to connect to. Note that your sandbox 
			// status, sending limits, and Amazon SES identity-related settings are specific to a given 
			// AWS region, so be sure to select an AWS region in which you set up Amazon SES. Here, we are using 
			// the US West (Oregon) region. Examples of other regions that Amazon SES supports are USEast1 
			// and EUWest1. For a complete list, see http://docs.aws.amazon.com/ses/latest/DeveloperGuide/regions.html 
			Amazon.RegionEndpoint REGION = Amazon.RegionEndpoint.USWest2;

			var credentials = new BasicAWSCredentials(Config.OutboundEmailServerConfig.AwsAccessKey, Config.OutboundEmailServerConfig.AwsSecretKey);

			// Instantiate an Amazon SES client, which will make the service call.
			AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(credentials, REGION);

			// Send the email.
			try
			{
				//Console.WriteLine("Attempting to send an email through Amazon SES by using the AWS SDK for .NET...");
				//client.SendEmail(request);
				client.SendRawEmail(request);
				//Console.WriteLine("Email sent!");
			}
			catch (Exception ex)
			{
				var exc = ex.ToString();
				//Console.WriteLine("The email was not sent.");
				//Console.WriteLine("Error message: " + ex.Message);
			}
		}
	}
}
