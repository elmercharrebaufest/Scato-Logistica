'begin VBS script code:

Call LogEntry()
Sub LogEntry()

'Force the script to finish on an error.
On Error Resume Next

'Declare variables
Dim objRequest
Dim Arg
Dim URL
Dim text
Set Arg = WScript.Arguments

if WScript.Arguments.Count = 0 then   
	WScript.Echo "Parametros faltantes" 
'The URL link.
end if
    URL = Arg(0)

Set objRequest = CreateObject("Microsoft.XMLHTTP")

'Open the HTTP request and pass the URL to the objRequest object
objRequest.open "GET", URL , false

'Send the HTML Request
objRequest.Send

status = objRequest.status
statusText = objRequest.statusText

WScript.StdOut.Write("Web: " + URL + Chr(13) & Chr(10))
WScript.StdOut.Write("Status: ")
WScript.StdOut.Write(status & vbCrLf)
WScript.StdOut.Write(statusText)

'Set the object to nothing
Set objRequest = Nothing

if status <> 200 then
    WScript.StdErr.Write("Error ejecutando la interface")
    WScript.StdErr.Write(status & vbCrLf)
    WScript.StdErr.Write(statusText)
    WScript.Quit status
end if

End Sub

'end VBS script code