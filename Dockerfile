FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app
COPY ./ShopM4/ShopM4.csproj ./ShopM4/
COPY ./ShopM4_DataMigrations/ShopM4_DataMigrations.csproj ./ShopM4_DataMigrations/
COPY ./ShopM4_Models/ShopM4_Models.csproj ./ShopM4_Models/
COPY ./ShopM4_Utility/ShopM4_Utility.csproj ./ShopM4_Utility/
COPY ./ShopM4.sln .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o release

WORKDIR /app/release
ENV Urls http://0.0.0.0:5000

CMD ["./ShopM4"]
