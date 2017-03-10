# Starten des Musterprojektes
1. Die Datei FahrtenbuchMusterprojekt...7z laden und entpacken.
2. Die Solution in Visual Studio 2015 öffnen (2013 geht nicht!). Dabei die Warnung, dass der Inhalt von einer Onlinequelle stammt, mit OK bestätigen.
3. Mit Build -> Rebuild Solution die Solution erzeugen.
3. Mit Test -> Window -> Text Explorer den Test `CheckDb` ausführen. Dieser wird fehlschlagen, er hat aber eine neue Datenbank mit dem Namen "Fahrtenbuch" angelegt.
4. Mit View -> SQL Server Object Explorer unter `(localdb)\MSSQLLocalDb`die Datenbank `FAHRTENBUCH_...` mit der rechten Maustaste anklicken und *New Query* im Kontextmenü wählen.
5. Den SQL Dump in Fahrtenbuch.sql in das Abfragefenster kopieren und ausführen (kleiner Play Button).

# Fahrzeugvermietung
Sie sollen für eine Fahrzeugvermietung eine Applikation entwickeln, die die Reservierung von Fahrzeugen abdeckt. Für die Speicherung der Daten gibt es schon eine Datenbank, das Modell ist unten abgebildet. Verwenden Sie das bereitgestellte Visual Studio 2015 Projekt samt der inkludierten Datenbank als Startpunkt der Entwicklung.

Die Kunden kommen auf 2 Arten zu ihrem Wagen: entweder sie suchen auf der Webseite nach bestimmten Auswahlkriterien wie Zeitraum, Anzahl der Sitzplätze, Hersteller oder Modell. Danach bekommen sie eine Liste von verfügbaren Autos angezeigt.
Die zweite Veriante ist die direkte Buchung im Geschäft. Hier ermittelt der Mitarbeiter alle verfügbaren Autos, und nach dem Kundengespräch wird ein Auto zugeteilt. Bei jeder Reservierung muss angegeben werden, wann das Auto zurückgebracht wird. Versäumt der Kunde diese Frist, so ist pro Tag ein Strafbetrag zu entrichten.

Bei jedem Auto gibt es einen Grundpreis, der immer zu entrichten ist. In diesem Grundpreis ist - je nach Wagen - ein gewisses Kontingent von Kilometern enthalten. Ist dieses aufgebraucht, dann wird pro 100 gefahrener Kilometer zusätzlich noch eine Gebühr aufgeschlagen.

## Datenmodell
![Datenmodell] (https://raw.githubusercontent.com/schletz/fachtheorie_1617/master/musterklausur/FahrtenbuchDb.png)

## Use Cases
Implementieren Sie die folgenden Use Cases. Jeder Usecase mittels Unittest auf die Richtigkeit des Codes zu prüfen.

### Use Case "Verfügbare Autos anzeigen"
Wie schon beschrieben kann der Kunde im Web das Angebot nach folgenden Kriterien filtern: Zeitraum (erster und letzter Tag der Reservierung), Anzahl der Sitzplätze, Hersteller und Modell. Dabei sind alle Suchfelder bis auf den Zeitraum optional. Entwickeln Sie eine Funktion, die eine Liste von verfügbaren Autos zu den Suchkriterien zurückliefert.

Ist kein Auto frei, dann soll die Suche intelligent agieren: Es wird zuerst der Filter nach dem Modell und dann der des Herstellers ignoriert. Finden sich mit dieser Lockerung freie Fahrzeuge, werden diese zurückgegeben. Nur wenn auch dies fehlschlägt, wird eine leere Liste geliefert.

*Tipp: Erstelle einen neuen Typ `SearchFilter`, der die Suchfelder als Properties speichert. Optionale Felder können mit ? nullable gesetzt werden. Implementiere die Methode dann in der Klasse FahrtenbuchModel, wobei sie den Typ `IEnumerable<Vehicle>` zurückliefert.*

### Use Case "Trip starten"
Wenn der Kunde das Fahrzeug reserviert hat, kann er es am Standort abholen. Der Mitarbeiter gibt hierfür das Kennzeichen des Wagens und seine Versicherungsnummer als persönliche ID ein. Danach wird der entsprechende Eintrag in der Tabelle Trips mit dem korrekten KM Stand zu Beginn der Fahrt angelegt.

### Use Case "Reservierung verlängern"
Der Kunde kann auch während der Reservierung anrufen, um die Reservierung zu verlängern. Er nennt dabei den neuen Tag, wann er das Auto zurückbringen wird. Das wird ihm aber nur gewährt, wenn das Auto auch nicht schon reserviert wurde. 

### Use Case "Auto zurückgeben"
Kommt der Kunde in die Filiale, so gibt der Mitarbeiter das Kennzeichen des Autos in eine Maske ein. Daraufhin erscheint dann der zu verlangende Preis. Entwickeln Sie eine Methode, die ausgehend vom Kennzeichen den Preis des gerade offenen Trips zurückgibt. Dieser ist daran zu erkennen, dass das Bis Datum NULL ist. In dieser Methode soll dann auch der Trip beendet und der Kilometerstand berichtigt werden.


