--
-- PostgreSQL database dump
--

-- Dumped from database version 11.6
-- Dumped by pg_dump version 11.6

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: questions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.questions (
    id_question integer NOT NULL,
    link_id integer,
    count_view integer DEFAULT 0,
    subject_id integer,
    user_id integer NOT NULL,
    CONSTRAINT questions_count_view_check CHECK ((count_view >= 0)),
    CONSTRAINT questions_link_id_check CHECK ((link_id >= 0))
);


ALTER TABLE public.questions OWNER TO postgres;

--
-- Name: questions_id_question_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.questions_id_question_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.questions_id_question_seq OWNER TO postgres;

--
-- Name: questions_id_question_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.questions_id_question_seq OWNED BY public.questions.id_question;


--
-- Name: statuses; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.statuses (
    id_status integer NOT NULL,
    name text NOT NULL
);


ALTER TABLE public.statuses OWNER TO postgres;

--
-- Name: statuses_id_status_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.statuses_id_status_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.statuses_id_status_seq OWNER TO postgres;

--
-- Name: statuses_id_status_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.statuses_id_status_seq OWNED BY public.statuses.id_status;


--
-- Name: subjects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.subjects (
    id integer NOT NULL,
    name text NOT NULL
);


ALTER TABLE public.subjects OWNER TO postgres;

--
-- Name: subject_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.subject_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.subject_id_seq OWNER TO postgres;

--
-- Name: subject_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.subject_id_seq OWNED BY public.subjects.id;


--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id_user integer NOT NULL,
    login text NOT NULL,
    age integer,
    status_id integer,
    CONSTRAINT users_age_check CHECK ((age >= 0))
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: users_id_user_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_id_user_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.users_id_user_seq OWNER TO postgres;

--
-- Name: users_id_user_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_id_user_seq OWNED BY public.users.id_user;


--
-- Name: questions id_question; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.questions ALTER COLUMN id_question SET DEFAULT nextval('public.questions_id_question_seq'::regclass);


--
-- Name: statuses id_status; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.statuses ALTER COLUMN id_status SET DEFAULT nextval('public.statuses_id_status_seq'::regclass);


--
-- Name: subjects id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subjects ALTER COLUMN id SET DEFAULT nextval('public.subject_id_seq'::regclass);


--
-- Name: users id_user; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN id_user SET DEFAULT nextval('public.users_id_user_seq'::regclass);


--
-- Data for Name: questions; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.questions VALUES (4, 5454, 60, 3, 36);
INSERT INTO public.questions VALUES (24, 435, 123, 2, 30);
INSERT INTO public.questions VALUES (25, 1313, 32, 9, 29);
INSERT INTO public.questions VALUES (26, 244, 39, 5, 30);
INSERT INTO public.questions VALUES (5, 244, 39, 5, 30);


--
-- Data for Name: statuses; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.statuses VALUES (1, 'новичок');
INSERT INTO public.statuses VALUES (3, 'хорошист');
INSERT INTO public.statuses VALUES (8, 'светило науки');
INSERT INTO public.statuses VALUES (9, 'профессор');
INSERT INTO public.statuses VALUES (7, 'почётный грамотей');
INSERT INTO public.statuses VALUES (6, 'учёный');
INSERT INTO public.statuses VALUES (5, 'отличник');
INSERT INTO public.statuses VALUES (10, 'главный мозг');
INSERT INTO public.statuses VALUES (4, 'умный');


--
-- Data for Name: subjects; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.subjects VALUES (2, 'English language');
INSERT INTO public.subjects VALUES (5, 'Chemistry');
INSERT INTO public.subjects VALUES (8, 'Mathematics');
INSERT INTO public.subjects VALUES (9, 'Geography');
INSERT INTO public.subjects VALUES (3, 'History');


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.users VALUES (33, 'lilyatomach', 36, 1);
INSERT INTO public.users VALUES (35, 'ttttebg', 32, 9);
INSERT INTO public.users VALUES (39, 'ohoho', 16, 8);
INSERT INTO public.users VALUES (40, 'nononn', 16, 9);
INSERT INTO public.users VALUES (34, 'alex', 19, 10);
INSERT INTO public.users VALUES (29, 'inf777', 16, 10);
INSERT INTO public.users VALUES (37, 'bebebe', 19, 3);
INSERT INTO public.users VALUES (32, 'qwerty', 45, 4);
INSERT INTO public.users VALUES (41, 'alex1', 54, 8);
INSERT INTO public.users VALUES (42, 'end_test', 23, 6);
INSERT INTO public.users VALUES (43, 'red', 22, 1);
INSERT INTO public.users VALUES (44, 'referrer', 54, 6);
INSERT INTO public.users VALUES (36, 'hahaha', 20, 3);
INSERT INTO public.users VALUES (28, 'teledima00', 8, 7);
INSERT INTO public.users VALUES (30, 'uh19', 30, 3);


--
-- Name: questions_id_question_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.questions_id_question_seq', 26, true);


--
-- Name: statuses_id_status_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.statuses_id_status_seq', 34, true);


--
-- Name: subject_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.subject_id_seq', 9, true);


--
-- Name: users_id_user_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_id_user_seq', 44, true);


--
-- Name: questions questions_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.questions
    ADD CONSTRAINT questions_pk PRIMARY KEY (id_question);


--
-- Name: statuses statuses_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.statuses
    ADD CONSTRAINT statuses_pk PRIMARY KEY (id_status);


--
-- Name: subjects subject_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subjects
    ADD CONSTRAINT subject_pk PRIMARY KEY (id);


--
-- Name: users users_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pk PRIMARY KEY (id_user);


--
-- Name: statuses_name_uindex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX statuses_name_uindex ON public.statuses USING btree (name);


--
-- Name: subject_subject_name_uindex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX subject_subject_name_uindex ON public.subjects USING btree (name);


--
-- Name: users_login_uindex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX users_login_uindex ON public.users USING btree (login);


--
-- Name: questions questions_subject_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.questions
    ADD CONSTRAINT questions_subject_id_fk FOREIGN KEY (subject_id) REFERENCES public.subjects(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: questions questions_users_id_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.questions
    ADD CONSTRAINT questions_users_id_user_fk FOREIGN KEY (user_id) REFERENCES public.users(id_user) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: users users_statuses_id_status_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_statuses_id_status_fk FOREIGN KEY (status_id) REFERENCES public.statuses(id_status) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- Name: TABLE statuses; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.statuses TO operator_workspace;
GRANT SELECT ON TABLE public.statuses TO quest;


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.users TO operator_workspace;
GRANT SELECT ON TABLE public.users TO quest;


--
-- PostgreSQL database dump complete
--

