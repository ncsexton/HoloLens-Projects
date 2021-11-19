# Universal Windows Platform Projects

 BasicClient_CXX is the basic client to be used with the base TCP server program.
 
 ClientUDP_Performance_CXX is the client used in a stress-test for the HoloLen's UDP performance.
 
 TCPHL2Tests contains two programs: "TCP Performance Test" is the TCP version of the aforementioned UDP performance stress-test. "Basic Communication Test" runs a simpler UWP application that communicates between the HoloLens and the provided server.
 
 The primary application layer for the socket code will be located in a file named "SocketStuff.h", "tcpSocketStuff.h", "udpSocketStuff.h", or "SocketData.h", depending on the project you are exploring.
 
 # How to build these projects

In Visual Studio 2019: 
  1) Build a new project of type "Blank App (Universal Windows - c++/CX)". Ensure that the project is named identical to whichever directory you are pulling code from.
  2) Copy and paste the contents of the <Grid> markup block from MainPage.xaml into the newly created project's MainPage.xaml
    - This will create the GUI
  3) Copy and paste the MainPage.xaml.cpp and MainPage.xaml.h into the newly created versions
  4) Open the newly created Package.appxmanifest, go to "Capabilities" and ensure "Internet (Client & Server"; "Internet (Client)"; and "Private Networks (Client & Server)" are checked.
  5) Build to HoloLens 2 in preferred manner.
    a) E.G. Set to Release, ARM64; and in Project/...Properties in the Debugging tab put the IPv4 of HoloLens 2 in "Machine Name" and ensure "Authentication Type" is "Universal (Unencrypted Protocl)"
