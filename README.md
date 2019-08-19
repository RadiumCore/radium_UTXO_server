Radium UTXO Server
===========================

A basic API server, that keeps a list of all unspent outputs in the radium blockchain. 

Endpoints: 

/api/utxo - provides sync progress information
/api/utxo/Xsomeradiumaddress - Jarray of unspent utxo's for that address


How to run
===========================
1) Set RPC connection information in utxo.conf
2) in project directory, run "dotnet run"

Dependency 
===========================
Requires dot net 2.2 SKD, available for windows here: https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.401-windows-x64-installer

