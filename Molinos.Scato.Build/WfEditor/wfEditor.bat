"C:\Program Files (x86)\WiX Toolset v3.8\bin\heat.exe" dir "..\..\Molinos.Scato.WfEditor\bin\Jenkins" -cg EditorFiles -gg -scom -sreg -sfrag -srd -dr INSTALLDIR -var var.EditorFilesDir -out "filesFragment.wxs"

"C:\Program Files (x86)\WiX Toolset v3.8\bin\candle" -dEditorFilesDir="..\..\Molinos.Scato.WfEditor\bin\Jenkins" wfEditor.bfdev.wxs filesFragment.wxs
"C:\Program Files (x86)\WiX Toolset v3.8\bin\light" wfEditor.bfdev.wixobj filesFragment.wixobj -o ..\..\Deploy\EditorWF\EditorWorkflows.bfdev.msi

"C:\Program Files (x86)\WiX Toolset v3.8\bin\candle" -dEditorFilesDir="..\..\Molinos.Scato.WfEditor\bin\Jenkins" wfEditor.dev.wxs filesFragment.wxs
"C:\Program Files (x86)\WiX Toolset v3.8\bin\light" wfEditor.dev.wixobj filesFragment.wixobj -o ..\..\Deploy\EditorWF\EditorWorkflows.dev.msi

"C:\Program Files (x86)\WiX Toolset v3.8\bin\candle" -dEditorFilesDir="..\..\Molinos.Scato.WfEditor\bin\Jenkins" wfEditor.prod.wxs filesFragment.wxs
"C:\Program Files (x86)\WiX Toolset v3.8\bin\light" wfEditor.prod.wixobj filesFragment.wixobj -o ..\..\Deploy\EditorWF\EditorWorkflows.prod.msi

"C:\Program Files (x86)\WiX Toolset v3.8\bin\candle" -dEditorFilesDir="..\..\Molinos.Scato.WfEditor\bin\Jenkins" wfEditor.qa.wxs filesFragment.wxs
"C:\Program Files (x86)\WiX Toolset v3.8\bin\light" wfEditor.qa.wixobj filesFragment.wixobj -o ..\..\Deploy\EditorWF\EditorWorkflows.qa.msi

"C:\Program Files (x86)\WiX Toolset v3.8\bin\candle" -dEditorFilesDir="..\..\Molinos.Scato.WfEditor\bin\Jenkins" wfEditor.cont.wxs filesFragment.wxs
"C:\Program Files (x86)\WiX Toolset v3.8\bin\light" wfEditor.qa.wixobj filesFragment.wixobj -o ..\..\Deploy\EditorWF\EditorWorkflows.cont.msi
