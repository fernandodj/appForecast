# appForecast


El primer paso para poder correr la aplicacion es ejecutar el script "creacionTablas.sql", en el cual se crean las tablas y se llenan con los datos necesarios para poder ejecutarla.

En el app.config se debe cambiar el item "connectionStrings" a la base de datos local de su PC para que se genere la conexión a su base de datos, especificamente el "data source" que contiene el nombre del servidor de la base de datos.
Ejemplo:
```
  <connectionStrings>
    <add name="ForecastEntities" connectionString="metadata=res://*/ForecastDataModel.csdl|res://*/ForecastDataModel.ssdl|res://*/ForecastDataModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=[NOMBRE DE CONEXION A CAMBIAR];initial catalog=Forecast;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
  </connectionStrings>
```
