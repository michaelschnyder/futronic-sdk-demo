# Futronic SDK Demo
A demo application integrating Futronic FS26

## Installation
You'll need to install the latest drivers downloaded from http://www.futronic-tech.com/download/ftrDriverSetup_win8_whql_3471.zip

You'll also need the libraries to access the device, which are stored under /lib
* ftrMFAPI.dll: Access to Mifare functionality of device
* ftrScanApi.dll: Access to scanner functionality

## File Versions
The application has been tested agains the following versions, these versions are included in the `lib`-Folder
* Windows Driver: Version 10.0.0.1, from http://www.futronic-tech.com/download.html
* Mifare Dll: Version 1.0.1.9 (from "WorkedExample" extracted from Free SDK)
* Scan Dll: Version 13.5.3315.1288 (from FinLogonPE, downloadable from http://www.futronic-tech.com/download/FinLogonPE_V7.4.3522.zip)

**Caution**: The official drivers installs an outdated and thus incompatible version of the Scan Dll to the `C:\Windows\System32`-Folder.
This seems not to be compatible with the driver, an updated version of the ScanDll can be found by installing the "FinLogon Personal Edition"
