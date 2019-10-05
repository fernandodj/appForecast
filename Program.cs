using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppClima
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Program prg = new Program();
            prg.createMenu();
            
        }

        private void createMenu()
        {
            int op;
            do
            {
                Console.Clear();
                Console.WriteLine("*************** ¿Que desea consultar? Ingrese el nro y presione 'Enter' ***************\n\n");
                Console.Write("1. ¿Cuantos períodos de sequía habrá?\n");
                Console.Write("2. ¿Cuantos períodos de lluvia habrá y qué día será el pico máximo de lluvia?\n");
                Console.Write("3. ¿Cuantos períodos de condiciones óptimas de presión y temperatura habrá?\n");
                Console.Write("4. Salir\n");
                op = Convert.ToInt32(Console.ReadLine());
                

                switch (op)
                {
                    case 1:
                        getPeriods("sequia");
                        Console.Write("Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;

                    case 2:
                        getPeriods("lluvia");
                        Console.Write("Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;

                    case 3:
                        getPeriods("optimo");
                        Console.Write("Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            } while (op != 4);
        }

        private void getPeriods(string clima = "")
        {
            List<Planets> listPlanets;
            ForecastEntities forecast = new ForecastEntities();
            listPlanets = forecast.Planets.ToList();
            calculatePeriods(listPlanets, clima, 10);
        }

        private void calculatePeriods(List<Planets> listPlanets, string weather, int years)
        {
            Planets pFerengi = listPlanets.Find(p => p.name == "Ferengi");
            Planets pVulcano = listPlanets.Find(p => p.name == "Vulcano");
            Planets pBetasoide = listPlanets.Find(p => p.name == "Betasoide");
            int days = 360 / pFerengi.velocity, periods = 0; //la variable days contiene los dias que tarda Ferengi en dar una vuelta completa, completando un año, tome este planeta para hacer los calculos, ya que es el mas lento
            string respuesta = String.Empty;

            switch (weather)
            {
                case "sequia":
                    periods = droughtPeriod(days, years, pFerengi, pVulcano, pBetasoide);
                    respuesta = "El clima " + weather + " tendra " + periods + " periodos.";
                    break;
                case "lluvia":
                    double[] periodMax = new double[3];
                    periodMax = rainPeriod(days, years, pFerengi, pVulcano, pBetasoide);
                    respuesta = "El clima " + weather + " tendra " + periodMax[0] + " periodos y el dia mas lluvioso sera el: " + periodMax[1] + "\nPerimetro: " + periodMax[2];
                    break;
                case "optimo":
                    periods = optimumPeriod(days, years, pFerengi, pVulcano, pBetasoide);
                    respuesta = "El clima " + weather + " tendra " + periods + " periodos.";
                    break;
            }

            Console.WriteLine(respuesta);
        }

        //Verifica si son colineales los planetas entre si y con el sol
        private int droughtPeriod(int days, int years, Planets pFerengi, Planets pVulcano, Planets pBetasoide)
        {
            Point ferengiPos, vulcanoPos, betasoidePos, sunPosition;
            sunPosition = new Point();
            sunPosition.x = 0;
            sunPosition.y = 0;
            int count = 0;

            for (int i = 0; i <= days * years; i++)
            {
                ferengiPos = calculatePlanetPosition(i, pFerengi);
                vulcanoPos = calculatePlanetPosition(i, pVulcano);
                betasoidePos = calculatePlanetPosition(i, pBetasoide);
                
                if (arePointsColineal(ferengiPos, vulcanoPos, betasoidePos, sunPosition))
                {
                    count++;
                }
            }

            return count;

        }

        //Verifica solo si son colineales los planetas entre si
        private int optimumPeriod(int days, int years, Planets pFerengi, Planets pVulcano, Planets pBetasoide)
        {
            Point ferengiPos, vulcanoPos, betasoidePos;
            int count = 0;

            for (int i = 0; i <= days * years; i++)
            {
                ferengiPos = calculatePlanetPosition(i, pFerengi);
                vulcanoPos = calculatePlanetPosition(i, pVulcano);
                betasoidePos = calculatePlanetPosition(i, pBetasoide);

                if (arePointsColineal(ferengiPos, vulcanoPos, betasoidePos))
                {
                    count++;
                }
            }
            return count;
        }

        private double[] rainPeriod(int days, int years, Planets pFerengi, Planets pVulcano, Planets pBetasoide)
        {
            Point ferengiPos, vulcanoPos, betasoidePos, sunPosition;
            int count = 0, dayMaxP = 0;
            double[] respuesta = new double[3];
            sunPosition = new Point();
            sunPosition.x = 0;
            sunPosition.y = 0;
            double perimeter = 0;

            for (int i = 0; i <= days * years; i++)
            {
                ferengiPos = calculatePlanetPosition(i, pFerengi);
                vulcanoPos = calculatePlanetPosition(i, pVulcano);
                betasoidePos = calculatePlanetPosition(i, pBetasoide);

                if(pointBelongsInTriangle(ferengiPos, vulcanoPos, betasoidePos, sunPosition))
                {
                    count++;
                    if(calculatePerimeter(ferengiPos, vulcanoPos, betasoidePos) > perimeter)
                    {
                        perimeter = calculatePerimeter(ferengiPos, vulcanoPos, betasoidePos);
                        dayMaxP = count;
                    }
                    
                }

            }

            respuesta[0] = count;
            respuesta[1] = dayMaxP;
            respuesta[2] = perimeter;
            return respuesta;
        }

        //Para calcular la posicion de los planetas, uso el calculo de coordenadas cartesianas basandome en el angulo que es del tipo x = r*cos(angulo)
        private Point calculatePlanetPosition(int day, Planets planet)
        {
            int degree = (day * planet.velocity) % 360;
            degree = degree < 0 ? degree + 360 : degree;
            double x = Math.Cos(degree * Math.PI / 180) * planet.sunDistance;
            double y = Math.Sin(degree * Math.PI / 180) * planet.sunDistance;
            Point pos = new Point();  

            pos.x = Math.Round(x, 2);
            pos.y = Math.Round(y, 2);

            return pos;
        }

        //Para ver si los puntos son colineales entre si, tengo que calcular la pendiente entre ambos, calculando las pendientes, las comparo y puedo verificar si pertenecen a una misma recta
        private bool arePointsColineal(Point a, Point b, Point c, Point d = null)
        {
            bool colineal = false;
            double pendAB = calculatePend(a, b);
            double pendAC = calculatePend(a, c);         
            if(d != null)
            {
                double pendAD = calculatePend(a, d);
                if(pendAB == pendAC && pendAB == pendAD)
                {
                    colineal = true;
                }
            } else
            {
                if (pendAB == pendAC)
                {
                    colineal = true;
                }
            }          
            return colineal;
        }

        private double calculatePend(Point a, Point b)
        {
            return (b.y - a.y) / (b.x - a.x);
        }

        //Para verificar que un punto este dentro de un triangulo, debo realizar el producto vectorial de los 3 puntos con el punto en cuestion, a traves del producto vectorial, si el signo de estos productos vectoriales es el mismo el punto se encuentra dentro del triangulo
        private bool pointBelongsInTriangle(Point a, Point b, Point c, Point point)
        {
            double VPab = vectorialProd(a, b, point);
            double VPcb = vectorialProd(b, c, point);
            double VPca = vectorialProd(c, a, point);

            return VPab > 0 && VPcb > 0 && VPca > 0 || VPab < 0 && VPcb < 0 && VPca < 0;
        }

        private double vectorialProd(Point a, Point b, Point c)
        {
            return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y);
        }

        //El perimetro de un triangulo es la suma de sus tres lados, los lados son las distancias entre los puntos
        private double calculatePerimeter(Point a, Point b, Point c)
        {
            double distanceAB = Math.Sqrt(Math.Pow((b.x - a.x), 2) + Math.Pow((b.y - a.y), 2));
            double distanceAC = Math.Sqrt(Math.Pow((c.x - a.x), 2) + Math.Pow((c.y - a.y), 2));
            double distanceBC = Math.Sqrt(Math.Pow((c.x - b.x), 2) + Math.Pow((c.y - b.y), 2));

            return distanceAB + distanceAC + distanceBC;

        }
    }
}
