#1. Übung: Properties

Es soll eine Schule, die mehrere Klassen hat, verwaltet werden. Eine Klasse hat wiederum mehrere Schüler. Dabei gelten folgende Grundprinzipien:
1. Die Id eines Schülers ist eindeutig.
2. Eine Klasse kann nicht mehr als 5 Schüler haben (zum Testen ist dies einfacher, in der Realität sind dies 30 Schüler).
3. Ein Schüler kann nur in genau 1 Klasse sein.

##  Klassendiagramm
Das Klassendiagramm in UML sieht wie folgt aus:
![UML Klassendiagramm](https://github.com/schletz/fachtheorie_1617/blob/master/uebung1/SchulVwKlassendiagramm3.png)

Implementiere nun die Methoden und Properties so, dass sie den oben genannten Prinzipien entsprechen. Dabei soll das Property *Klasse* des Schülers auch beschreibbar sein. Allerdings darf durch das Setzen des Properties die Regel mit der Klassenhöchstzahl nicht verletzt werden.

## Hinweise zur Umsetzung
Lege eine neue Solution mit dem Namen SchulVw an. Gib die 3 modellierten Klassen in ein Modul mit dem Namespace SchulVw.Model. Zum Testen lege in dieser Solution zudem noch eine Konsolenapplikation SchulVw.App an.
