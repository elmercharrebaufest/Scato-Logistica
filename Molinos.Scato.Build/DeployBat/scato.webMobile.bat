cd /d %~dp0
START cmd.exe /k "Molinos.Scato.WebMobile.deploy.cmd /Y -setParamFile:%~dp0WebMobile\<ambiente>.DeployParameters.xml"