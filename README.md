
# Algemene beschrijving applicatie  
  
### Studentnummer:  60399
### Student naam:  Chris Buter
  De Multy client chat app is een chat app waarmee mensen met een server kunnen verbinden en berichten heen en weer sturen op **TCP** niveau. De app bestaad uit een client en server gedeelte die bestaan uit 2 WPF's.

De server is instaad om een naam aan te geven om bijvoorbeeld een chatroom aan te maken. Hierbij kan de server een port en buffersize meegeven. De port wordt gebruikt voor het aanzetten van de server via een TCP listener. Deze luistert op elk IP adress op de aangegeven port adress. De buffersize wordt gebruikt voor het ontvangen van berichten.

De client kan verbinden met de server via een TCP client. Hiervoor moet de gebruiker een naam, ipadress, port en buffer size meesturen. De port en ipadress wordt gebruikt om te verbinden met de server. De naam die meegestuurd is wordt gebruikt om de client te herkennen voor de andere verbonden clients.

De server understeund meerdere clients tegelijkertijd en kan zelf ook berichten versturen naar alle verbonden clients. Mocht de connectie verbroken worden bij de client of server wordt dit opgevangen en moet bijde partijen opnieuw verbinden.

## Generics  

### Beschrijving van concept in eigen woorden  
Met generics kan je een datatype aangeven aan methodes en variables. Dit kan gebruikt worden met collecties door een datatype mee te geven. Door dit te doen help je de performance te verbeteren. Dit komt doordat je zonder gebruik van generics een object moet casten naar het gewenste datatype. Dit heet boxing en un-boxing. Wanneer generics hier wordt toegepast is dit niet meer nodig doordat je al hebt aangegeven welk datatype wordt verwacht en de collectie zal alleen die datatype accepteren. 
### Code voorbeeld van je eigen code  

    List<NetworkStream> clients = new List<NetworkStream>();
    private List<NetworkStream> GetClients()
    {
	    return clients;
    }
Dit stukje code laat zien hoe een generic lijst wordt aangemakt en gebruikt. Dit stukje code werdt gebruikt als een getter van de lijst om elke keer de nieuwste versie van de lijst op te halen. Dit is later uit de code gehaald. 

De generic is te zien door wat tussen **< >** staat. Hier Gebruik ik de **Networkstream** object om te defineren welke datatypes in de lijst te verwachten is. In de functie zelf moet ook weer de **generic** gedefineerd worden om ook de functie effecienter te laten maken. Dit betekend dat er nu niet meer **gecast** moet worden naar een **Networkstream** object wanneer de lijst wordt opgevraagt en de items gebruikt worden.
### Alternatieven & adviezen  
Je kan ook een ArrayList gebruiken. Hier wordt het mogelijk om niet een datatype aan te geven en zo verwacht de arraylist alleenmaar objecten. Omdat alles een object is c# is dit mogelijk. Je krijgt dan telkens ook een object terug 
```
ArrayList myAL = new ArrayList();
      myAL.Add("Hello");
      myAL.Add("World");
      myAL.Add("!");
```
`foreach ( Object obj in myList )`
`{` 
`Console.Write( "   {0}", obj );`
`Console.WriteLine();`
`}`
In dit voorbeeld is te zien dat een string telkens weergegeven wordt door een object op te roepen.
### Authentieke en gezaghebbende bronnen  

 1. https://docs.microsoft.com/en-us/dotnet/api/system.collections.arraylist?view=netframework-4.7.
 2. https://www.geeksforgeeks.org/c-sharp-generics-introduction/
  
## Boxing & Unboxing
### Beschrijving van concept in eigen woorden    
Zoals net al benoemd is boxing en unboxing het concept van een waarde uit een object halen. dit wordt voornamelijk gedaan door **casting**. Dit betekend dat een string 1 kan omgezet worden naar een Int32 met de waarde 1; Met het vorige voorbeeld van generics is te zien dat bijvoorbeeld een String gebruikt wordt vanuit een lijst van objecten. Dit object wordt dan omgezet naar een string om het te kunnen weergeven. Dit is zwaar voor de performance. 
### Code voorbeeld van je eigen code  
In de laatste versie van het product is er te zien dat boxing en unboxing wordt gedaan tijdens het versturen van een bericht. Een string wordt in eerste instantie verstuurd maar om dit te kunnen doen wordt de string omgezet in een byte array dat verstuurd wordt via de stream van de client.  Dit wordt gedaan door de getBytes methode.
```
byte[] bytesToSend = Encoding.ASCII.GetBytes($"Verbonden met:{serverNameBox.Text}");
            NetworkStream currentStream = currentClient.GetStream();
            currentStream.Write(bytesToSend, 0, bytesToSend.Length);
```
Ook tijdens het ontvangen van een bericht wordt boxing en unboxing gedaan. Hier wordt de byte array omgezet naar een leesbaar bericht (String). Dit stukkje code is wat slimmer dan het verzenden van een bericht. Hier kan je met een buffer het hele bericht inladen. Het uiteindelijke bericht wordt omgezet naar een string. Dit keer via de toString() methode.
```
StringBuilder responseData = new StringBuilder();
            Console.WriteLine($"RECIEVE MESSAGE: Buffer size : {bufferSize}");
            byte[] buffer = new byte[bufferSize];
            do
            {
                int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                responseData.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, readBytes));
            } while (networkStream.DataAvailable);
            return responseData.ToString();
```
### Alternatieven & adviezen  
Een alternatief is dus generics. Zoals eerder uitgelegt worden datatypes niet meer omgezet naar objecten en visa versa omdat je dit aangeeft via het gebruik van een generic. 
### Authentieke en gezaghebbende bronnen  
  
##Delegates & Invoke  
###Beschrijving van concept in eigen woorden  
### Code voorbeeld van je eigen code  
AddMessageDelegate newMessage = new AddMessageDelegate(AddMessage);
###Alternatieven & adviezen  
###Authentieke en gezaghebbende bronnen  
  
##Threading & Async  
###Beschrijving van concept in eigen woorden  
###Code voorbeeld van je eigen code  
###Alternatieven & adviezen  
###Authentieke en gezaghebbende bronnen
