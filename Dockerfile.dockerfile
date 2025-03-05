# Використовуємо образ SDK для побудови
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Копіюємо файл проєкту і встановлюємо залежності
COPY ToDoListProj/ToDoListProj.csproj ./ToDoListProj/
RUN dotnet restore ToDoListProj/ToDoListProj.csproj

# Копіюємо всі файли проєкту та публікуємо додаток
COPY ToDoListProj/ ./ToDoListProj/
RUN dotnet publish ToDoListProj/ToDoListProj.csproj -c Release -o out

# Створюємо фінальний образ на основі ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Відкриваємо порт 80 для додатку
EXPOSE 80

# Запускаємо програму
ENTRYPOINT ["dotnet", "ToDoListProj.dll"]
