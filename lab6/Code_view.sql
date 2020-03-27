create table films(
    id serial not null primary key,
    Name text not null
); --таблица фильмов
create table Actors(
    id serial not null primary key,
    Name text not null
); --таблица актёров
create table films_of_actors(
    id_actor int not null references Actors on DELETE CASCADE on UPDATE cascade,
    id_film int not null references films on DELETE CASCADE on UPDATE cascade,
    honorarium int not null default 0 check (honorarium>0),
    primary key (id_actor,id_film)
); --фильмы, в которых снимался актёр

create or replace view view_cinema as
select A.Name as actor_name,f.Name as film_name,honorarium from films_of_actors
inner join Actors A on films_of_actors.id_actor = A.id
inner join films f on films_of_actors.id_film = f.id; --создание представления

create or replace function func_for_insert() returns trigger as $$
    declare
        id_film_par int;
        id_actor_par int;
    begin

        select id into id_actor_par from actors where Name = new.actor_name;                  --ищём id актёра
        select id into id_film_par from films where Name = new.film_name;                     --ищём id кинофильма
        if id_actor_par is null or id_film_par is null then
            if id_film_par is null and id_actor_par notnull                                       --фильма нету, а актёр есть
                then
                    insert into films(Name) values (new.film_name) returning id into id_film_par; --вставляем новый фильм и записываем id в id_fil_par
            elsif id_film_par notnull  and  id_actor_par is null                                  --фильм есть, актёра нету
                then
                    insert into actors(Name) VALUES (new.actor_name) returning id into id_actor_par; --вставляем нового актёра и записываем id в id_actor_par
            elsif id_film_par is null and id_film_par is null                                        --если никого нету
                then
                    insert into films(Name) VALUES (new.film_name) returning id into id_film_par;     --вставляем фильм и записываем id в id_film_par
                    insert into  actors(Name) values (new.actor_name) returning id into id_actor_par; --вставляем актёра и записываем id в id_actor_par
            end if;
            insert INTO films_of_actors(id_actor, id_film,honorarium) VALUES (id_actor_par, id_film_par,new.honorarium);--вставляем id_actor_par и id_film_par в связующую таблицу
        else
            if not exists(select * from films_of_actors where id_actor = id_actor_par and id_film = id_film_par) then
                insert into films_of_actors(id_actor, id_film, honorarium) VALUES (id_actor_par,id_film_par,new.honorarium);
            else
                update films_of_actors set honorarium = honorarium + new.honorarium where id_actor = id_actor_par and id_film = id_film_par;
            end if;
        end if;
        return new;
    end
    $$ language plpgsql;

create or replace function func_for_delete() returns trigger as $$
    begin
        delete from films_of_actors --удаляем
        where id_actor = (select id from actors where Name = old.actor_name) and id_film = (select id from films where Name = old.film_name);
        return old;
    end;
    $$ language plpgsql;

create or replace function func_for_update() returns trigger as $$
    declare
        id_film_par int;
        id_actor_par int;
        count_duplicates int = 0;
    begin
        select id into id_film_par from films where Name = new.film_name;    --ищем название нового фильма
        select id into id_actor_par from actors where Name = new.actor_name; --ищем имя нового актёра
        select count(*)+1 into count_duplicates from view_cinema where actor_name = new.actor_name and film_name = new.film_name;
        if (id_actor_par is null or id_film_par is null or count_duplicates = 1) then
            if (id_actor_par is null) then                                       --если актёра нету в бд
                update actors set Name = new.actor_name where id = (select id from Actors where Name = old.actor_name); -- обновляем имя актёра
            end if;
            if (id_film_par is null) then                                                     --если фильма нет в бд
                update films set Name = new.film_name where id = (select id from films where Name = old.film_name);
            end if;
            update films_of_actors set id_actor = id_actor_par,id_film = id_film_par,honorarium = new.honorarium --обновляем запись
            where id_actor = (select id from actors where Name = old.actor_name) and id_film = (select id from films where Name = old.film_name);
        else
            update films_of_actors set honorarium = honorarium+new.honorarium where id_actor = id_actor_par and id_film = id_film_par;
            delete from view_cinema
            where actor_name = old.actor_name and
                film_name = old.film_name;
       end if;
        return new;
    end
    $$ language plpgsql;
create trigger "insert_som" INSTEAD OF INSERT on view_cinema for each row execute procedure func_for_insert();
create trigger "delete_som" instead of DELETE on view_cinema for each row execute procedure func_for_delete();
create trigger "update_som" instead of update on view_cinema for each row execute procedure func_for_update();
