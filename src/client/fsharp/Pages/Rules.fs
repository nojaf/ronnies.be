module Ronnies.Client.Pages.Rules

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Feliz
open Ronnies.Client.Styles
open Ronnies.Client.Components.Page

let private RulesPage =
    React.functionComponent
        ("RulesPage",
         (fun () ->
             let lis text = li [] [ span [] [ str text ] ]
             let faqItem q a = [ dt [] [ str q ]; dd [] [ str a ] ]

             page [] [
                 h1 [] [ str "Manifesto" ]
                 ol [ classNames [ Bootstrap.Roman
                                   Bootstrap.My3 ] ] [
                     lis "Je drinkt graag Rodenbach (deuh)."
                     lis "Je voegt juiste mo permanente cafes/restaurants toe."
                     lis "Je voegt enkel e plekke toe, aj doa ter plekke 1 ant kippen zit."
                     lis "Je checkt ofdat de locatie froa juste is GPS-wise."
                     lis "Je typt een opmerking at een opmerking waard is."
                     lis "Je meugt e plekke toevoegen aj in bende zit, aj gie em zet."
                     lis "Je mag na 15 minuten wachten toch een plekke toevoegen aj gie em niet gezet et."
                     lis "Je meugt gin emoji's in de naam zetten"
                 ]
                 h3 [ ClassName "mb-2" ] [ str "FAQ" ]
                 dl [] [
                     yield! faqItem "Kzitn met e misse?" "Een admin kan alles rechttrekken, ping nojaf."
                     yield! faqItem
                                "Me GPS zegt dak e bitje ernaast zitten."
                                "Je kut bi het ingeven zelve nog op de koarte klikken, tis de R van Ronny die telt."
                     yield! faqItem "Woarom de term 'Ronny'?" "Gin idee, Svenne zegt dat lik. Oe goat dat ol."
                     yield! faqItem
                                "Meug je een trouw toevoegen?"
                                "Nej, je moet onder normale omstandigheden, de toegevoegde plekke de weke derop kunnen bezoeken."
                     yield! faqItem "En meugen winkels?" "Nope, enkel permanente cafes/restaurants"
                 ]
             ]))

exportDefault RulesPage
