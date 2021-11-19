#include <WinSock2.h>
#include <WS2tcpip.h>
#include <iostream>
//see this page if wanting to use a header which offers useful IP handling/information
// https://docs.microsoft.com/en-us/windows/win32/winsock/creating-a-basic-winsock-application
#pragma comment(lib, "Ws2_32.lib")

/*Server
- Init Winsock
- Create a Socket
- Bind the Socket
- Listen for Client
- Accept connection from Client
- Communicate using Socket
- Disconnect
*/
/*Client
- Init Winsock
- Create a Socket
- Connect to Server
- Communicate using Socket
- Disconnect
*/

#define DEF_PORT "40666"
#define BUFFER_SIZE 512

int main()
{
	//Initialize the dynamic library so that function calls can actually be made
	//	Doubles to verify that the functionality is present.
	WSADATA wsaData;
	int result;
	if ((result = WSAStartup(MAKEWORD(2, 2), &wsaData)) != 0)//the function call loads the DLL, make word is version number
	{
		std::cerr << "Could not initialize/use Windows Sockets: " << result << " returned\n";
		return -1;
	}

	//Create a Socket
	struct addrinfo* resultInfo = NULL, * ptr = NULL, hints;

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;//IPv4 address family
	hints.ai_socktype = SOCK_STREAM;//stream socket for TCP
	hints.ai_protocol = IPPROTO_TCP;//TCP protocol
	hints.ai_flags = AI_PASSIVE;//socket address struct will be used during binding
	result = getaddrinfo(NULL, DEF_PORT, &hints, &resultInfo);
	if (result != 0)
	{
		std::cerr << "Could not get local address and port for the server:"
			<< result << " returned\n";
		WSACleanup();
		return -2;
	}//else success on gather the local address and port to use with the server, so create Socket
	SOCKET listener = INVALID_SOCKET;
	listener = socket(resultInfo->ai_family, resultInfo->ai_socktype, resultInfo->ai_protocol);
	if (listener == INVALID_SOCKET)
	{
		std::cerr << "Could not create socket: Last error was " << WSAGetLastError() << "\n";
		freeaddrinfo(resultInfo);
		WSACleanup();
		return -3;
	}

	//Bind the Socket to a network address within the system
	result = bind(listener, resultInfo->ai_addr, (int)resultInfo->ai_addrlen);
	if (result == SOCKET_ERROR)
	{
		std::cerr << "Could not bind the socket: Last error was " << WSAGetLastError() << "\n";
		freeaddrinfo(resultInfo);
		closesocket(listener);
		WSACleanup();
		return -4;
	}
	freeaddrinfo(resultInfo);//address information no longer need - C-style free up

	//Listen for Client as the Socket is Bound to a Port
	if (listen(listener, SOMAXCONN) == SOCKET_ERROR)
	{
		std::cerr << "Could not listen for a client: Last error was " << WSAGetLastError() << "\n";
		closesocket(listener);
		WSACleanup();
		return -5;
	}//else we are waiting for a client

	//Accept Connection to Client
	// Note the information for normal communications mentioned
	//		https://docs.microsoft.com/en-us/windows/win32/winsock/accepting-a-connection
	SOCKET client = INVALID_SOCKET;
	client = accept(listener, NULL, NULL);
	if (client == INVALID_SOCKET)
	{
		std::cerr << "Could not connection client: Last error was " << WSAGetLastError() << "\n";
		closesocket(listener);
		WSACleanup();
		return -6;
	}//else the client connection was established

	//Communicate with Client
	char receive[BUFFER_SIZE];
	int sendResult;
	int bufferLength = BUFFER_SIZE;

	do {//communicate until client closes connection
		result = recv(client, receive, bufferLength, 0);
		if (result > 0)
		{
			std::cout << "Received char count: " << result << "\n";
			//Echo back
			sendResult = send(client, receive, result, 0);
			if (sendResult == SOCKET_ERROR)
			{
				std::cerr << "Could not echo back: Last error was " << WSAGetLastError() << "\n";
				closesocket(client);
				WSACleanup();
				return -7;
			}//else data was echoed back
			std::cout << "Sent char count: " << sendResult << "\n";
		}
		else if (result == 0)
			std::cout << "Connection will now close\n";
		else
		{
			std::cerr << "Could not receive from client: Last error was " << WSAGetLastError() << "\n";
			closesocket(client);
			WSACleanup();
			return -8;
		}

	} while (result > 0);

	//Disconnect Server
	result = shutdown(client, SD_SEND);
	if (result == SOCKET_ERROR)
	{
		std::cerr << "Could not shutdown client: Last error was " << WSAGetLastError() << "\n";
		closesocket(client);
		WSACleanup();
		return -9;
	}
	closesocket(client);
	WSACleanup();
	char wait = 'c';
	std::cin >> wait;
	return 0;
}