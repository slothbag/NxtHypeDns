NxtHypeDns
==========

Update: DNSChains now support .hype name resolution in addition to .nxt for Nxt clearnet names and .bit for namecoin names. Recommend users switch to that instead of this.

A small standalone DNS resolver that will lookup hype domain names using Nxt Aliases.

Nxt aliases must have a prefix (or namespace) of 4973.

The simplest way to get up and running is to use DNSMASQ (a great tool anyway) to redirect .hype name resolution to the server running Nxt and NxtHypeDns.

NxtHypeDns listens on port 1053, so the line server=/hype/127.0.0.1#1053 in your dnsmasq.conf should be enough.

Make sure Nxt and NxtHypeDns are both running. Currently its hardcoded to expect Nxt and NxtHypeDns on the same machine.

Thats it, your network should be able to resolve hype names with no client changes.  Nxt Aliases take about a minute to register and cost about 0.03 cents.

Obviously you will need Mono to run this on linux.  "mono NxtHypeDns.exe"

Dependencies
* ARSoft.Tools https://arsofttoolsnet.codeplex.com/ 
* Json.NET http://james.newtonking.com/json
