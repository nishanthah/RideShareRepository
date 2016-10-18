:: Build Simphony in release mode
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe RideShareMobileApp.sln /p:Configuration=Release

::Generate signed APK file 
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe RideShare\RideShare.Droid\RideShare.Droid.csproj /p:Configuration=Release /t:SignAndroidPackage