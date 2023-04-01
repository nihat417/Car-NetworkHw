
using Server.Commands;
using Server.Enums;
using Server.Models;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var cars = new List<Car>();
var countcar= cars.Count;
var path = @"..\..\..\FakeDatas\CarsData.json";
var ip = IPAddress.Parse("127.0.0.1");
var port = 12345;


var tcplistner = new TcpListener(ip,port);
tcplistner.Start(10);



if(File.Exists(path))
{
    var fakedatas=JsonSerializer.Deserialize<List<Car>>(File.ReadAllText(path))!;
    cars=fakedatas;
}
else
{
    throw new Exception("not found path");
}



bool Add(Car car)
{
    if (car is not null)
    {
        cars.Add(car);
        return true;
    }

    return false;
}

bool Delete(int id)
{
    if (cars.Count > 0 )
    {
        foreach (var car in cars)
        {
            if (car.Id == id)
            {
                cars.Remove(car);
                return true;
            }
        }

        return false;
    }

    return false;
}


Car? GetById(int id)
{
    if (cars.Count > 0)
        foreach (var car in cars)
            if (car.Id == id)
                return car;

    return null;
}




while (true)
{
    var client = tcplistner.AcceptTcpClient();
    Console.WriteLine($"Client {client.Client.RemoteEndPoint} accepted.");
    var stream = client.GetStream();
    var br = new BinaryReader(stream);
    var bw = new BinaryWriter(stream);

    while (true)
    {
        var jsonStr = br.ReadString();
        var command = JsonSerializer.Deserialize<Command>(jsonStr);
        Console.WriteLine(command?.Method);

        if (command is null) continue;

        switch (command.Method)
        {

            case HttpMethods.GET:
                {
                    if (command.Car is not null)
                    {
                        int carId = command.Car.Id;
                        var getCar = GetById(carId);
                        if (getCar != null)
                        {
                            var response = JsonSerializer.Serialize(getCar);
                            bw.Write(response);
                        }
                        else bw.Write(JsonSerializer.Serialize(new Car()));
                    }
                    break;
                }

            case HttpMethods.POST:
                if (command.Car is not null)
                {
                    command.Car.Id = ++countcar;
                    bw.Write(Add(command.Car));
                    Console.WriteLine(cars.Count.ToString());
                }
                else bw.Write(false);
                break;

            case HttpMethods.DELETE:
                if (command.Car is not null)
                {
                    int carId = command.Car.Id;
                    if(Delete(carId)==true)
                    {
                        var response = JsonSerializer.Serialize(Delete(carId));
                        var newcarslist= JsonSerializer.Serialize(cars);
                        File.WriteAllText(path, newcarslist);
                        bw.Write(response);
                    }
                    else
                    {
                        bw.Write(false);
                        Console.WriteLine("bele masn yoxdu");
                    }
                    
                }
                
                break;
            default:
                break;
        }
    }
}
