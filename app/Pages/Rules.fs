module Rules

open Feliz
open React
open React.Props

[<ReactComponent>]
let RulesPage () =
    let lis text = li [] [ span [] [ str text ] ]
    let faqItem q a = [ dt [] [ str q ] ; dd [] [ str a ] ]

    main [ Id "manifesto" ] [
        h1 [] [ str "Manifesto" ]
        ol [] [
            lis "Je drinkt graag Rodenbach (deuh)."
            lis "Je voegt juiste mo permanente cafes/restaurants toe."
            lis "Je voegt enkel e plekke toe, aj doa ter plekke 1 ant kippen zit."
            lis "Je checkt ofdat de locatie froa juste is GPS-wise."
            lis "Je typt een opmerking at een opmerking waard is."
            lis "Je meugt e plekke toevoegen aj in bende zit, aj gie em zet."
            lis "Je mag na 15 minuten wachten toch een plekke toevoegen aj gie em niet gezet et."
            lis "Je meugt gin emoji's in de naam zetten"
        ]
        h2 [] [ str "FAQ" ]
        dl [] [
            yield! faqItem "Kzitn met e misse?" "Een admin kan alles rechttrekken, ping nojaf."
            yield!
                faqItem
                    "Me GPS zegt dak e bitje ernaast zitten."
                    "Je kut bi het ingeven zelve nog op de koarte klikken, tis de R van Ronny die telt."
            yield! faqItem "Woarom de term 'Ronny'?" "Gin idee, Svenne zegt dat lik. Oe goat dat ol."
            yield!
                faqItem
                    "Meug je een trouw toevoegen?"
                    "Nej, je moet onder normale omstandigheden, de toegevoegde plekke de weke derop kunnen bezoeken."
            yield! faqItem "En meugen winkels?" "Nope, enkel permanente cafes/restaurants"
            yield!
                faqItem
                    "Oewe, zim op nieuw begun mss?"
                    "Jom, seizoen twee gom zeggen. Tis behoorlijk lik de vorige keer. Mo nu kuj wel met meerderen een nieuwe plekke toevoegen."
            yield!
                faqItem
                    "Is de oude data er nog?"
                    "Joas, der is een double back-up, mo der wordt momenteel niet mee gedoan."
            yield!
                faqItem
                    "En oe werkt de punten telling nu?"
                    "Iedereen die genoteerd stond bij het aanmaken van een plekke krijgt een punt."
        ]
    ]
