cd /d %~dp0
START cmd.exe /k "Molinos.Scato.Workflow.deploy.cmd /Y -setParamFile:%~dp0Workflow\<ambiente>.DeployParameters.xml"