#pragma once

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <Windows.h>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include <vector>
#include <fstream>
#include <chrono>
#include <string>
#include <stdlib.h>

#pragma comment(lib, "Ws2_32.lib")

#define DEF_PORT "40666"
#define BUFFER_SIZE 0X40000//0X3FFFF
#define HOST_IP "34.75.16.157"

constexpr uint32_t SHIFT_ITR = 10;
constexpr uint32_t ITERATIONS = 100;

std::string attempt()
{
	WSADATA wsaData;

	int result;
	result = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (result != 0)
	{
		//Communicate->Text = "Could not load Windows Socket DLL: " + result;
		return "failure";
	}

	//Build Socket
	struct addrinfo* resultInfo = NULL, * ptr = NULL, hints;
	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;

	result = getaddrinfo("127.0.0.1", DEF_PORT, &hints, &resultInfo);
	if (result != 0)
	{
		//Communicate->Text = "Could not get the address information: " + result;
		WSACleanup();
		return "failure";
	}

	SOCKET connectMe = INVALID_SOCKET;
	ptr = resultInfo;
	connectMe = socket(ptr->ai_family, ptr->ai_socktype, ptr->ai_protocol);

	if (connectMe == INVALID_SOCKET)
	{
		//Communicate->Text = "Could not create socket: Last error was " + WSAGetLastError();
		freeaddrinfo(resultInfo);
		WSACleanup();
		return "failure";
	}

	//Connect Socket
	result = connect(connectMe, ptr->ai_addr, (int)ptr->ai_addrlen);
	if (result == SOCKET_ERROR)
	{
		closesocket(connectMe);
		connectMe = INVALID_SOCKET;
	}

	freeaddrinfo(resultInfo);
	if (connectMe == INVALID_SOCKET)
	{
		//Communicate->Text = "Could not connect to server: Might not be an err, might be occupied address";
		WSACleanup();
		return "failure";
	}

	//Communicate with Server
	std::vector<char> receive(BUFFER_SIZE);//char receive[BUFFER_SIZE];
	//const char* message = "Hello Server\n";
	std::vector<char> message(BUFFER_SIZE);//char message[BUFFER_SIZE];
	for (uint32_t i = 0; i < (BUFFER_SIZE - 1); ++i)
	{
		//Cycle through the printable ASCII characters
		message[i] = static_cast<char>((i % 95) + 32);
	}
	message[BUFFER_SIZE - 1] = '\0';
	int bufferLength = BUFFER_SIZE;

	//Information transmission time, there and back again.
	std::chrono::time_point<std::chrono::steady_clock> startTime, endTime;
	uint32_t counter = 0;
	uint32_t array_counter = 0;
	uint32_t dT[SHIFT_ITR][ITERATIONS] = { 0 };
	int cur_len = BUFFER_SIZE;
	//	Wait for server to finish closing the connection
	do
	{
		//Initial message sent
		startTime = std::chrono::steady_clock::now();

		//adding first 4 bytes as length
		uint32_t length = static_cast<uint32_t>(cur_len);
		std::memcpy(&message[0], &cur_len, 4);
		

		result = send(connectMe, &message[0], cur_len, 0);
		if (result == SOCKET_ERROR)
		{
			//std::cerr << "Could not send message: Last error was " << WSAGetLastError() << "\n";
			closesocket(connectMe);
			return "Could not send message";// -5;
		}
		result = recv(connectMe, &receive[0], bufferLength, 0);
		if (result > 0)
		{
			endTime = std::chrono::steady_clock::now();
			//std::cout << "Bytes received: " << result << "\n";
			if (counter < ITERATIONS)
			{
				dT[array_counter][counter] = std::chrono::duration_cast<std::chrono::microseconds>(endTime - startTime).count();
				counter++;
			}
			else
			{
				if (array_counter < (SHIFT_ITR - 1))
				{
					cur_len >>= 1;
					message.resize(cur_len);
					message.back() = '\0';
					counter = 0;
					array_counter++;
				}
				else
				{
					result = shutdown(connectMe, SD_SEND);
					if (result == SOCKET_ERROR)
					{
						//std::cerr << "Could not shutdown connection request: Last error was " << WSAGetLastError() << "\n";
						closesocket(connectMe);
						WSACleanup();
						return "Could not shutdown connection request";//-6;
					}
				}
			}
		}
		else if (result == 0);// std::cout << "Connection closed\n";
		else return "Could not receive message: Last error was ...";// +WSAGetLastError() + "\n";
	} while (result > 0);

	//Disconnect Client
	closesocket(connectMe);
	WSACleanup();

	/*std::ofstream file(filepath + "\\data.csv", std::ios::out);
	if (file)
	{
		for (uint32_t i = 0; i < SHIFT_ITR; ++i)
		{
			file << "\nMB_TIME, " << (BUFFER_SIZE >> i) << "\n";
			for (uint32_t j = 0; j < ITERATIONS; ++j)
			{
				//std::cout << "time[" << i << "]: " << dT[i] << '\n';
				file << dT[i][j] << ',';
			}
		}
	}*/
	//else return "file not created!\n";

	//return filepath;// "Testing Concluded Successfully";
	return 0;
}
