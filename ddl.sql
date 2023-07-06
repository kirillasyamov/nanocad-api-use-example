create table point (
    id serial primary key,
    x float,
    y float,
    z float
);

create table polyline (
    id serial primary key,
    first int,
    last int,
    foreign key (first) references point(id),
    foreign key (last) references point(id)
);
