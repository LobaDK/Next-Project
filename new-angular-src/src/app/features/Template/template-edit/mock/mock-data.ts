import { QuestionnaireTemplateEditor, QuestionType, TemplateStatus } from "../../../../shared/models/template-edit.model";

export const QuestionnaireTemplateEditorMock: QuestionnaireTemplateEditor = {
  title: "Evaluering af SKP-elever",
  description: "Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD",
  status: TemplateStatus.Draft,
  questions: [
    {
      id: "q1",
      type: QuestionType.RadioGroup,
      prompt: "Indlæringsevne",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Viser lidt eller ingen forståelse for arbejdsopgaverne." },
        { value: 2, label: "Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden." },
        { value: 3, label: "Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden." },
        { value: 4, label: "Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden." },
        { value: 5, label: "Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden." }
      ]
    },
    {
      id: "q2",
      type: QuestionType.RadioGroup,
      prompt: "Kreativitet og selvstændighed",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Viser intet initiativ. Er passiv, uinteresseret og uselvstændig." },
        { value: 2, label: "Viser ringe initiativ. Kommer ikke selv med løsningsforslag. Viser ingen interesse i at tilrettelægge eget arbejde." },
        { value: 3, label: "Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde." },
        { value: 4, label: "Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde." },
        { value: 5, label: "Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre." }
      ]
    },
    {
      id: "q3",
      type: QuestionType.RadioGroup,
      prompt: "Arbejdsindsats",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Uacceptabel" },
        { value: 2, label: "Under middel" },
        { value: 3, label: "Middel" },
        { value: 4, label: "Over middel" },
        { value: 5, label: "Særdeles god" }
      ]
    },
    {
      id: "q4",
      type: QuestionType.RadioGroup,
      prompt: "Orden og omhyggelighed",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig." },
        { value: 2, label: "Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed." },
        { value: 3, label: "Påpasselighed og omhyggelighed middel. Rimelig god orden." },
        { value: 4, label: "Meget påpasselig både i praktik og teori. God orden." },
        { value: 5, label: "I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden." }
      ]
    },

    {
      id: "q8",
      type: QuestionType.RadioGroup,
      prompt: "Mødestabilitet",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Du møder ikke hver dag til tiden." },
        { value: 2, label: "Du møder næsten hver dag til tiden." },
        { value: 3, label: "Du møder hver dag til tiden." }
      ]
    },
    {
      id: "q9",
      type: QuestionType.RadioGroup,
      prompt: "Sygdom",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Du melder ikke afbud ved sygdom." },
        { value: 2, label: "Du melder, for det meste afbud, når du er syg." },
        { value: 3, label: "Du melder afbud, når du er syg." }
      ]
    },
    {
      id: "q10",
      type: QuestionType.RadioGroup,
      prompt: "Fravær",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Du har et stort fravær." },
        { value: 2, label: "Du har noget fravær." },
        { value: 3, label: "Du har stort set ingen fravær." },
        { value: 4, label: "Du har ingen fravær." }
      ]
    },
    {
      id: "q11",
      type: QuestionType.RadioGroup,
      prompt: "Praktikpladssøgning",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Du søger ingen praktikpladser." },
        { value: 2, label: "Du ved, at du skal søge alle relevante praktikpladser, men det kniber med handlingen." },
        { value: 3, label: "Du søger alle relevante praktikpladser, men skal have hjælp til at søge praktikpladser, der ligger længere væk end i din bopælskommune." },
        { value: 4, label: "Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune." },
        { value: 5, label: "Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune. Du søger også praktikplads inden for en anden uddannelse, som dit GF giver adgang til." }
      ]
    },
    {
      id: "q12",
      type: QuestionType.RadioGroup,
      prompt: "Synlighed",
      allowOtherComment: false,
      otherLabel: null,
      options: [
        { value: 1, label: "Du har ikke en synlig profil på praktikpladsen.dk." },
        { value: 2, label: "Du skal ofte påmindes om at synliggøre din profil på praktikpladsen.dk." },
        { value: 3, label: "Du har altid en synlig, men ikke opdateret profil på praktikpladsen.dk." },
        { value: 4, label: "Du har altid en opdateret og synlig profil på praktikpladsen.dk." }
      ]
    }
  ],
};