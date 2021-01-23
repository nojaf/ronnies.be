import { createElement } from "../../../_snowpack/pkg/react.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { ofSeq, ofArray } from "../.fable/fable-library.3.1.1/List.js";
import { page } from "../Components/Page.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { classNames } from "../Styles.js";
import { append, delay } from "../.fable/fable-library.3.1.1/Seq.js";

function RulesPage() {
    const lis = (text) => react.createElement("li", {}, react.createElement("span", {}, text));
    const faqItem = (q, a) => ofArray([react.createElement("dt", {}, q), react.createElement("dd", {}, a)]);
    return page([], [react.createElement("h1", {}, "Manifesto"), react.createElement("ol", keyValueList([classNames(["roman", "my-3"])], 1), lis("Je drinkt graag Rodenbach (deuh)."), lis("Je voegt juiste mo permanente cafes/restaurants toe."), lis("Je voegt enkel e plekke toe, aj doa ter plekke 1 ant kippen zit."), lis("Je checkt ofdat de locatie froa juste is GPS-wise."), lis("Je typt een opmerking at een opmerking waard is."), lis("Je meugt e plekke toevoegen aj in bende zit, aj gie em zet."), lis("Je mag na 15 minuten wachten toch een plekke toevoegen aj gie em niet gezet et."), lis("Je meugt gin emoji\u0027s in de naam zetten")), react.createElement("h3", {
        className: "mb-2",
    }, "FAQ"), react.createElement("dl", {}, ...ofSeq(delay(() => append(faqItem("Kzitn met e misse?", "Een admin kan alles rechttrekken, ping nojaf."), delay(() => append(faqItem("Me GPS zegt dak e bitje ernaast zitten.", "Je kut bi het ingeven zelve nog op de koarte klikken, tis de R van Ronny die telt."), delay(() => append(faqItem("Woarom de term \u0027Ronny\u0027?", "Gin idee, Svenne zegt dat lik. Oe goat dat ol."), delay(() => append(faqItem("Meug je een trouw toevoegen?", "Nej, je moet onder normale omstandigheden, de toegevoegde plekke de weke derop kunnen bezoeken."), delay(() => faqItem("En meugen winkels?", "Nope, enkel permanente cafes/restaurants"))))))))))))]);
}

export default (() => createElement(RulesPage, null));

