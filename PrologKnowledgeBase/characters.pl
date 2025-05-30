% Character definitions
% Harry Potter universe
character(harry_potter).
character(hermione_granger).
character(ron_weasley).
character(dumbledore).
character(voldemort).
character(snape).
character(hagrid).
character(sirius_black).
character(remus_lupin).
character(ginny_weasley).
character(draco_malfoy).
character(bellatrix_lestrange).
character(neville_longbottom).
character(luna_lovegood).
character(cedric_diggory).

% Lord of the Rings universe
character(frodo_baggins).
character(samwise_gamgee).
character(gandalf).
character(aragorn).
character(legolas).
character(gimli).
character(boromir).
character(gollum).
character(sauron).
character(saruman).
character(galadriel).
character(arwen).
character(éowyn).
character(faramir).
character(denethor).

% Game of Thrones universe
character(tyrion_lannister).
character(daenerys_targaryen).
character(jon_snow).
character(arya_stark).
character(sansa_stark).
character(cersei_lannister).
character(jaime_lannister).
character(tywin_lannister).
character(oberyn_martell).
character(margaery_tyrell).
character(joffrey_baratheon).
character(ramsay_bolton).
character(theon_greyjoy).
character(brienne_of_tarth).
character(bronn).

% Questions and their answers for each character
% Harry Potter characters
has_property(harry_potter, wears_glasses, yes).
has_property(harry_potter, has_scar, yes).
has_property(harry_potter, is_wizard, yes).
has_property(harry_potter, is_student, yes).
has_property(harry_potter, is_gryffindor, yes).
has_property(harry_potter, is_male, yes).
has_property(harry_potter, is_orphan, yes).
has_property(harry_potter, has_patronus, yes).
has_property(harry_potter, is_half_blood, yes).
has_property(harry_potter, is_chosen_one, yes).

has_property(hermione_granger, wears_glasses, no).
has_property(hermione_granger, has_scar, no).
has_property(hermione_granger, is_wizard, yes).
has_property(hermione_granger, is_student, yes).
has_property(hermione_granger, is_gryffindor, yes).
has_property(hermione_granger, is_male, no).
has_property(hermione_granger, is_orphan, no).
has_property(hermione_granger, has_patronus, yes).
has_property(hermione_granger, is_muggle_born, yes).
has_property(hermione_granger, is_chosen_one, no).

% Lord of the Rings characters
has_property(frodo_baggins, is_hobbit, yes).
has_property(frodo_baggins, is_ring_bearer, yes).
has_property(frodo_baggins, is_male, yes).
has_property(frodo_baggins, is_warrior, no).
has_property(frodo_baggins, is_magical, no).
has_property(frodo_baggins, is_royalty, no).
has_property(frodo_baggins, is_immortal, no).
has_property(frodo_baggins, is_evil, no).
has_property(frodo_baggins, has_ring, yes).
has_property(frodo_baggins, is_main_character, yes).

has_property(gandalf, is_hobbit, no).
has_property(gandalf, is_ring_bearer, no).
has_property(gandalf, is_male, yes).
has_property(gandalf, is_warrior, yes).
has_property(gandalf, is_magical, yes).
has_property(gandalf, is_royalty, no).
has_property(gandalf, is_immortal, yes).
has_property(gandalf, is_evil, no).
has_property(gandalf, has_ring, no).
has_property(gandalf, is_main_character, yes).

% Game of Thrones characters
has_property(tyrion_lannister, is_dwarf, yes).
has_property(tyrion_lannister, is_royalty, yes).
has_property(tyrion_lannister, is_male, yes).
has_property(tyrion_lannister, is_warrior, no).
has_property(tyrion_lannister, is_magical, no).
has_property(tyrion_lannister, is_immortal, no).
has_property(tyrion_lannister, is_evil, no).
has_property(tyrion_lannister, is_main_character, yes).
has_property(tyrion_lannister, is_hand_of_king, yes).
has_property(tyrion_lannister, is_imp, yes).

has_property(daenerys_targaryen, is_dwarf, no).
has_property(daenerys_targaryen, is_royalty, yes).
has_property(daenerys_targaryen, is_male, no).
has_property(daenerys_targaryen, is_warrior, yes).
has_property(daenerys_targaryen, is_magical, yes).
has_property(daenerys_targaryen, is_immortal, no).
has_property(daenerys_targaryen, is_evil, no).
has_property(daenerys_targaryen, is_main_character, yes).
has_property(daenerys_targaryen, has_dragons, yes).
has_property(daenerys_targaryen, is_mother_of_dragons, yes).

% Rules for guessing characters
guess_character(Answers, Character) :-
    character(Character),
    check_answers(Answers, Character).

check_answers([], _).
check_answers([(Question, Answer)|Rest], Character) :-
    has_property(Character, Question, Answer),
    check_answers(Rest, Character).

% Add new character
add_character(Name, Properties) :-
    assertz(character(Name)),
    add_properties(Name, Properties).

add_properties(_, []).
add_properties(Name, [(Question, Answer)|Rest]) :-
    assertz(has_property(Name, Question, Answer)),
    add_properties(Name, Rest).

% Get all possible questions
get_all_questions(Questions) :-
    findall(Question, 
            (has_property(_, Question, _)), 
            Questions1),
    sort(Questions1, Questions).

% Get next question based on current answers
get_next_question(Answers, NextQuestion) :-
    get_all_questions(AllQuestions),
    find_most_discriminating_question(Answers, AllQuestions, NextQuestion).

% Find the most discriminating question
find_most_discriminating_question(Answers, Questions, BestQuestion) :-
    findall(Question-Score,
            (member(Question, Questions),
             \+ member((Question, _), Answers),
             calculate_discrimination_score(Question, Answers, Score)),
            QuestionScores),
    sort(2, @>=, QuestionScores, SortedScores),
    SortedScores = [BestQuestion-_|_].

% Calculate how well a question discriminates between remaining characters
calculate_discrimination_score(Question, Answers, Score) :-
    findall(Character,
            (character(Character),
             check_answers(Answers, Character)),
            RemainingCharacters),
    length(RemainingCharacters, Total),
    findall(Character,
            (member(Character, RemainingCharacters),
             has_property(Character, Question, yes)),
            YesAnswers),
    length(YesAnswers, YesCount),
    NoCount is Total - YesCount,
    Score is abs(YesCount - NoCount).

% Check if character can be uniquely identified with current answers
can_identify_character(Answers, Character) :-
    findall(C,
            (character(C),
             check_answers(Answers, C)),
            [Character]).

% Get all characters that match current answers
get_matching_characters(Answers, Characters) :-
    findall(Character,
            (character(Character),
             check_answers(Answers, Character)),
            Characters). 
