using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ARSoft.Tools.Net.Dns;
using Newtonsoft.Json.Linq;

namespace NxtHypeDns
{
	class Program
	{
		public static void Main(string[] args)
		{
			DnsServer server = new DnsServer(new IPEndPoint(IPAddress.Any, 1053), 10, 10, ProcessQuery);
			
			server.ExceptionThrown += new EventHandler<ExceptionEventArgs>(server_ExceptionThrown);
			server.Start();
		
			Console.Write("Press any key to quit . . . \n");
			Console.ReadKey(true);
			
			server.Stop();
		}
		
		public static DnsMessage ProcessQuery(DnsMessageBase qquery, IPAddress clientAddress, ProtocolType protocol)
		{
			DnsMessage query = qquery as DnsMessage;
			query.IsQuery = false;
			Boolean bFound = true;
			IPAddress tmpadd = null;
	
			Console.WriteLine(query.Questions[0].RecordType + " Query for: " + query.Questions[0].Name);
			
			if ((query.Questions.Count == 1) && (query.Questions[0].RecordType == RecordType.Aaaa))
			{
				if (query.Questions[0].Name.EndsWith(".hype", true, null)) {
					string trimmed_alias = query.Questions[0].Name.ToLower().Substring(0, query.Questions[0].Name.Length - 5);
					string address = LookupAlias("4973" + trimmed_alias);
					if (address != "") {
						try {
							tmpadd = IPAddress.Parse(address);
						}
						catch (Exception ex) {
							Console.WriteLine("Invalid IP address");
							bFound = false;
						}
					}
					else
						bFound = false;
				}
				else {
				    bFound = false;
				}
			}
			else
				bFound = false;

			if (bFound) {
				query.ReturnCode = ReturnCode.NoError;
				query.AnswerRecords.Add(new AaaaRecord(query.Questions[0].Name, 3600, tmpadd));
			}
			else
			{
				query.ReturnCode = ReturnCode.ServerFailure;
			}
	
			//Console.WriteLine(query.AnswerRecords.Count);
			//Console.WriteLine(tmpadd.ToString());
			return query;
		}
	
		// Capture any DNS server exceptions
		static void server_ExceptionThrown(object sender, ExceptionEventArgs e)
	    {
	        Console.WriteLine(e.Exception.ToString());
	    }
		
		//Call the NXT API getAlias
		public static string LookupAlias(string alias)
		{
			Console.WriteLine("Looking up alias: " + alias);
			
			var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:7876/nxt");
			httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			httpWebRequest.Method = "POST";
			
			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string postdata = "requestType=getAlias&aliasName=" + alias;
				streamWriter.Write(postdata);
			}
			
			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				var responseText = streamReader.ReadToEnd();
				JObject response = null;
				try {
					response = JObject.Parse(responseText);
				}
				catch (Exception ex) {
					return "";
				}
				
				JToken address;
				if (response.TryGetValue("aliasURI", out address))
				    return address.ToString();
				else
					return "";
			}
		}
	}
}