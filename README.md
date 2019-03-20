
# Multy-Client-Chat-App
An app where multy clients can comunicate back and forth
# Algemene beschrijving applicatie  
  
### Studentnummer:  60399
### Student naam:  Chris Buter
  De Multy client chat app is een chat app waarmee mensen met een server kunnen verbinden en berichten heen en weer sturen op **TCP** niveau. De app bestaad uit een client en server gedeelte die bestaan uit 2 WPF's.

De server is instaad om een naam aan te geven om bijvoorbeeld een chatroom aan te maken. Hierbij kan de server een port en buffersize meegeven. De port wordt gebruikt voor het aanzetten van de server via een TCP listener. Deze luistert op elk IP adress op de aangegeven port adress. De buffersize wordt gebruikt voor het ontvangen van berichten. De buffersize kan verschillen voor de client en de server.	

De client kan verbinden met de server via een TCP cliënten. Hiervoor moet de gebruiker een naam, IP adres, port en buffer size meesturen. De port en ipadres wordt gebruikt om te verbinden met de server. De naam die meegestuurd is wordt gebruikt om de cliënt te onderscheiden van de andere verbonden cliënt.

De server ondersteund meerdere clients tegelijkertijd en kan zelf ook berichten versturen naar alle verbonden clients. Mocht de connectie verbroken worden bij de client of server wordt dit opgevangen en moet beide partijen opnieuw verbinden.
# Beschrijving termen en code
> De code voorbeelden die hieronder beschreven zijn. Zijn niet in de nieuwste versie van het product te vinden maar je kan ze terugvinden op oudere versies van de applicatie op mijn repo in Github. Zie link: https://github.com/wintercomo/Multy-Client-Chat-App
## Generics  

### Beschrijving van concept in eigen woorden  
Met generics kan je een datatype aangeven aan methodes en variables. Dit kan ook gebruikt worden met collecties door een datatype aan een collectie mee te geven. Door dit te doen help je de performance te verbeteren doordat er minder gecast hoeft te worden naar andere datatypes. Zonder gebruik van generics moet wordt een datatype omgezet naar een object dat later weer terug wordt gezet naar het gewenste datatype. Dit heet boxing en un-boxing. Wanneer generics wordt toegepast is Gebeurt dit niet meer doordat je al hebt aangegeven welk datatype wordt verwacht. Bij een generic collectie zal de collectie alleen die datatype accepteren en retourneren. 
### Code voorbeeld van je eigen code  
Dit stukje code laat zien hoe een generic lijst wordt aangemaakt en gebruikt. Dit stukje code werd gebruikt als een getter van de lijst om elke keer de nieuwste versie van de lijst op te halen. Dit is later uit de code gehaald. 

De generic is te zien door wat tussen **< >** staat. Hier Gebruik ik de **Networkstream** object om te defineren welke datatypes in de lijst te verwachten is. 

    List<NetworkStream> clients = new List<NetworkStream>();
    private List<NetworkStream> GetClients()
    {
	    return clients;
    }
iedere keer als je een methode creëert dat een **generic lijst retuneerd** moet de de datatype in de methode ook definiëren. In het code fragment is dit te zien. Dit betekend dat er nu niet meer **gecast** moet worden naar een **Networkstream** object wanneer de lijst wordt opgevraagt en de items gebruikt worden.
### Alternatieven & adviezen  
Je kan ook een **ArrayList** gebruiken. Hier wordt het niet mogelijk om een datatype aan te geven en zo verwacht de arraylist alleen maar objecten. Omdat alles een object is c# is dit mogelijk. Je krijgt dan telkens ook een object terug 
```
ArrayList myAL = new ArrayList();
      myAL.Add("Hello");
      myAL.Add("World");
      myAL.Add("!");
```
```
foreach ( Object obj in myList )
{
	Console.Write( "   {0}", obj );
	Console.WriteLine();`
}
```

In dit voorbeeld is te zien dat een string telkens weergegeven wordt door een object op te roepen. Dit komt omdat het object **gecast** wordt naar een string. Dit is een **alternatief** maar zeker **geen advies**. Dit is zwaar voor de performance doordat er continu boxing en unboxing plaatsvind. Door het **expliciete casten** van een object naar een string creëer men veel **overhead** waardoor **performance** achteruit gaat.
### Authentieke en gezaghebbende bronnen  

 - https://docs.microsoft.com/en-us/dotnet/api/system.collections.arraylist?view=netframework-4.7.
 - https://www.geeksforgeeks.org/c-sharp-generics-introduction/
  
## Boxing & Unboxing
### Beschrijving van concept in eigen woorden    
Zoals net al is benoemd is boxing en unboxing het concept van een datatype opslaan als object (Boxing) en dit later weer omzetten naar een specifiek datatype (Unboxing). Dit wordt voornamelijk gedaan door **casting**. Dit betekend dat een string 1 kan omgezet worden naar een Int32 met de waarde 1. Met het vorige voorbeeld van generics is te zien dat een String gebruikt wordt vanuit een lijst van objecten. Dit object wordt dan omgezet naar een string om het te kunnen weergeven. Dit heet **expliciete casting** en dit is zwaar voor de performance doordat  het veel overhead veroorzaakt tijdens runtime.
### Code voorbeeld van je eigen code  
Een simpel voorbeeld van dit concept het omzetten van een StringBuilder object naar een string. Dit is te zien in het ontvangen van data methode. Hier wordt stukjes van het bericht aan elkaar gezet door de string builder dat uiteindelijk het volledige berricht behoudt. Wanneer het bericht compleet is wordt dit omgezet door een toString() methode dat aan ieder object van C# gekoppeld is. 
```
return responseData.ToString();
```
> toString() is vertaald na compile naar:
> string s = `string.Concat(object, object)`
> Hierdoor wordt eerst de ResponseData in een object omgezet door boxing. Hierna wordt her omgezet naar een String via un-boxing

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
Een alternatief is dus generics. Zoals eerder benoemd worden datatypes niet meer omgezet naar objecten en visa versa als je gebruik maakt van generics. Dit zal ook veel verschil maken in performance doordat er geen 2 verschillende datatypes onthouden worden en er ook niet meer gecast hoeft te worden. Met casten kunnen ook makkelijk exceptions optreden als het casten mislukt.  
### Authentieke en gezaghebbende bronnen  
  
 - https://www.google.com/search?client=firefox-b-d&ei=4tVuXNq2H8jPwQLYoK34Cg&q=boxing+and+unboxing+in+c%23&oq=boxing+and+unb&gs_l=psy-ab.3.0.0i203l10.4427.6791..8429...0.0..0.215.1354.6j5j1....2..0....1..gws-wiz.......0i71j0j35i39j0i67j0i131i67j0i131.6R6Mctitoso
 - https://davidzych.com/when-doing-string-concatenation-in-c-why-dont-you-have-to-call-tostring-on-non-string-variables/

## Delegates & Invoke  
### Beschrijving van concept in eigen woorden  
Een **delegate** is een pointer functie dat verwijst naar de locatie van iets anders. Dit kan goed gebruikt worden om tijdens **multythreading** om weer terug te springen naar de main (UI) thread.  Dit heet **thread marshalling**. Door een delegate te gebruiken kan je verwijzen naar een methode die je in de main thread wilt aanroepen. Wanneer de delegate wordt aangeroepen wordt er uit de thread gesprongen. Door **Invoke** te gebruiken kan de aangewezen methode oproepen van de gewenste thread. Het mooie aan delegates is dat ze "Type save" zijn. Dit is ook goed voor performance doordat je niet vanalles aan kan koppelen maar alleen dat wat aangegeven is.
### Code voorbeeld van je eigen code  

> Dit voorbeeld laat niet zien hoe invoke gebruikt wordt doordat dit niet gebruikt is in mijn project. Dit is nog wel te vinden in een oude versie op GitHub

Met deze regel code definieer ik de delegate. Hier geef ik aan dat de delegate een void methode kan aanwijzen die een string verwacht als parameter. 
`delegate void AddMessageDelegate(string n);`
Door de delegate te inistialiseren met de AddMessage methode heb ik nu een variable, genaamt 'newMessage', die ik kan aanroepen die verwijst naar de AddMessage methode. Als ik nu newMessage aanroep met een string die ik wil toevoegen aan de chat, Zal de AddMessage opgeroepen worden in de Main (UI) Thread met de gewenste string.
```
public void ReceiveData()
{
try
{
int i;
string s;
byte[] data = new byte[1024];
AddMessageDelegate newMessage = new AddMessageDelegate(AddMessage);
newMessage("Connected!");
// Code continues ...
```
Dit stuk code laat zien hoe je de **delegate** kan oproepen via de **invoke** methode. Dit is niet handig doordat je een object moet meegeven dat voor de functie niet nodig is. 

> De _items variable is de lijst met berichten. Deze lijst bestaat niet meer in de nieuwere versie. Dit kon ook als: 
> `this.Invoke(addMessage, new Object(), {message});`
```
if (this.chatBox.InvokeRequired)
{
AddMessageDelegate addMessage = new AddMessageDelegate(AddMessage);
// this is the delegate
this.Invoke(addMessage, _items, message);
}
else
{
// code continues ...
```
### Alternatieven & adviezen  
Wat gebruikt kan worden als alternatief voor **delegates** zijn **lamda expressies**. Hiermee kan je gelijk een functie aangeven dat uitgevoerd kan worden. In plaats wijzen naar een methode kan je gelijk een methode definiëren. Hierdoor hoef je ook geen gebruik meer te maken van de invoke methode. 
### Authentieke en gezaghebbende bronnen  
https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions
https://www.google.com/search?client=firefox-b-d&q=alternative+to+delegate+c%23
  
## Threading & Async  
### Beschrijving van concept in eigen woorden 
Het concept van **threads** is waar de code op loopt. Alle code wordt **parallel** uitgevoerd op 1 thread en dat is de main (UI) thread. Op deze thread wordt alles synchroon uitgevoerd. Hierdoor loopt alles achter elkaar en kan het programma vastlopen omdat het wacht tot een taak klaar is. Zware taken zou je liever **asynchroon** willen oplossen zodat de main thread nooit hoeft te wachten en dat het gewenste resultaat opgehaald kan worden wanneer het zo ver is. Dit kan je doen op verschillende manieren.
### Code voorbeeld van je eigen code  
Dit is de functie die aangeroepen wordt wanneer een gebruiker wilt verbinden. Het aanmaken van de thread is onderaan het fragment te zien. De Recieve data kijkt continue voor een bericht en wanneer er een bericht is zal de methode het bericht afhandelen. Zonder een nieuwe thread aan te maken zal het programma vastlopen tot er een bericht binnen is. Door dit asynchroon te laten lopen door de methode op een aparte thread te runnen zal de main (UI) thread niet vastlopen doordat het wacht voor een bericht. Dit komt omdat dit nu gebeurt op een andere thread die nu wacht op een bericht. Dit wordt gedaan door een thread aan te maken met de methode **ThreadStart()** met de RecieveData methode als **parameter**. Een probleem hiermee is dat er geen parameter met de RecieveData meegegeven kan worden.  Dit kan opgelost woorden door een **lamda expressie** te gebruiken in plaats de **ThreadStart();**
```
private void BtnConnect(object sender, EventArgs e)
{
try
{
_items.Add("connecting...");
chatBox.DataSource = null;
chatBox.DataSource = _items;
Console.WriteLine("Trying to connect");
String server = "127.0.0.1";
Int32 port = 9000;
tcpClient = new TcpClient(server, port);
// Create and start a thread
thread = new Thread(new ThreadStart(ReceiveData));
thread.Start();
```
> De thread met een lamda expressie en parameter: 
> ```
> thread = new Thread(()=> RecieveData(**SomeParameter**));
> thread.Start();
> ```
### Alternatieven & adviezen  
Een alternatief voor theads zijn **Tasks** en **AsyncAwait**. Door een task aan te maken in plaats van een thread laat je het werk binnen in de task asynchroon werken. omdat een task uiteindelijk ook weer op een thread draait. Met tasks gebruikt je niet meer de **Thread.start** methode maar de **Task.Run** methode. Nadelen van Tasks zijn dat je geen **returns** kan gebruiken binnen in een task. Een task verwacht een taak uit te voeren en zelf niks teruggeven. Voordelen zijn dat je de **status** van een task kan checken en gebruiken om **status afhankelijk methodes** op te roepen. Met Async en Await kan je aan het systeem vertellen om te wachten op een asynchrone methode. Wanneer je **await** voor een asynchrone methode zet wordt die methode eigenlijk in een **task** die draait op de **background thread**. Dit wordt meestal gebruikt tijdens netwerk verkeer zoals een verbinding maken met een chatroom.
### Authentieke en gezaghebbende bronnen
https://docs.microsoft.com/en-us/dotnet/csharp/async
https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=netframework-4.7.2
