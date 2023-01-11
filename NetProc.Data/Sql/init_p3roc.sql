BEGIN TRANSACTION;

-- Table: Machine
INSERT INTO Machine (Id, MachineType, NumBalls, DisplayMonitor) VALUES (1, 'PDB', 4, 0);

-- Table: Coils
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-0', 'flipperLwRMain', 30, '', 0, 0);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-1', 'flipperLwRHold', 30, '', 0, 0);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-2', 'flipperLwLMain', 30, '', 0, 0);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-3', 'flipperLwLHold', 30, '', 0, 0);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-4', 'trough', 30, '', 0, 0);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-5', 'sling_l', 30, '', 0, 20);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-6', 'sling_r', 30, '', 0, 20);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-7', 'bumper_1', 30, '', 0, 20);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-8', 'bumper_2', 30, '', 0, 20);
INSERT INTO Coils (Number, Name, PulseTime, Bus, Polarity, Search) VALUES ('A0-B0-9', 'bumper_3', 30, '', 0, 20);

-- Table: Leds
INSERT INTO Leds (Number, Name, Bus, Polarity, Discriminator) VALUES 
('A0-R0-G1-B2', 'start', '', 0, 0),
('A0-R3-G4-B5', 'shooter', '', 0, 0),
('A0-R6-G7-B8', 'unused_0', '', 0, 0),
('A0-R9-G10-B11', 'unused_1', '', 0, 0),
('A0-R12-G13-B14', 'unused_2', '', 0, 0),
('A0-R15-G16-B17', 'unused_4', '', 0, 0),
('A0-R18-G19-B20', 'unused_5', '', 0, 0),
('A0-R21-G22-B23', 'unused_6', '', 0, 0);

-- Table: Switches
INSERT INTO Switches (Number, Name, Type, Tags, SearchReset, SearchStop) VALUES 

-- board 0/A
('00', 'coin1', 0, 'door', '', ''),
('01', 'coin2', 0, 'door', '', ''),
('02', 'coin3', 0, 'door', '', ''),
('03', 'coinDoor', 1, 'door', '', 'open'),
('04', 'exit', 0, 'door', '', ''),
('05', 'down', 0, 'door', '', ''),
('06', 'up', 0, 'door', '', ''),
('07', 'enter', 0, 'door', '', ''),
-- board 0/B
('08', 'slamTilt', 0, 'tilt', '', ''),
('09', 'tilt', 0, 'tilt', '', ''),
('10', 'start', 0, '', '', ''), 
('11', 'flipperLwL', 0, '', '', 'closed'),
('12', 'flipperLwR', 0, '', '', 'closed'),
('13', 'ball_shooter', 0, '', '', 'closed'),
('14', 'not_used_14', 0, '', '', ''),
('15', 'not_used_15', 0, '', '', ''),
-- board 1/A
('16', 'trough1', 0, 'trough', '', ''),
('17', 'trough2', 0, 'trough', '', ''),
('18', 'trough3', 0, 'trough', '', ''),
('19', 'trough4', 0, 'trough', '', ''),
('20', 'shooter_lane', 0, 'shooter', '', ''),
('21', 'outlane_r', 0, '', 'open', ''),
('22', 'inlane_r', 0, '', 'open', ''),
('23', 'scoop_r', 0, '', '', 'closed'),
-- board 1/B
('24', 'inlane_l', 0, '', 'open', ''),
('25', 'outlane_l',0, '', 'open', ''),
('26', 'sling_l', 0, 'sling', 'open', ''),
('27', 'sling_r', 0, 'sling', 'open', ''),
('28', 'not_used_28', 0, '', '', ''),
('29', 'not_used_29', 0, '', '', ''),
('30', 'not_used_30', 0, '', '', ''),
('31', 'not_used_31', 0, '', '', ''),
-- board 2/A
('32', 'not_used_32', 0, '', '', ''),
('33', 'not_used_33', 0, '', '', ''),
('34', 'not_used_34', 0, '', '', ''),
('35', 'not_used_35', 0, '', '', ''),
('36', 'not_used_36', 0, '', '', ''),
('37', 'not_used_37', 0, '', '', ''),
('38', 'not_used_38', 0, '', '', ''),
('39', 'not_used_39', 0, '', '', ''),
-- board 2/B
('40', 'not_used_40', 0, '', '', ''),
('41', 'not_used_41', 0, '', '', ''),
('42', 'not_used_42', 0, '', '', ''),
('43', 'not_used_43', 0, '', '', ''),
('44', 'not_used_44', 0, '', '', ''),
('45', 'bumper_1', 0, 'bumper', 'open', ''),
('46', 'bumper_2', 0, 'bumper', 'open', ''),
('47', 'bumper_3', 0, 'bumper', 'open', ''),
-- board 3/A
('48', 'not_used_48', 0, '', '', ''),
('49', 'not_used_49', 0, '', '', ''),
('50', 'not_used_50', 0, '', '', ''),
('51', 'not_used_51', 0, '', '', ''),
('52', 'not_used_52', 0, '', '', ''),
('53', 'not_used_53', 0, '', '', ''),
('54', 'not_used_54', 0, '', '', ''),
('55', 'not_used_55', 0, '', '', ''),
-- board 3/B
('56', 'not_used_56', 0, '', '', ''),
('57', 'not_used_57', 0, '', '', ''),
('58', 'not_used_58', 0, '', '', ''),
('59', 'not_used_59', 0, '', '', ''),
('60', 'not_used_60', 0, '', '', ''),
('61', 'not_used_61', 0, '', '', ''),
('62', 'not_used_62', 0, '', '', ''),
('63', 'not_used_63', 0, '', '', '');

-- PLAYERS
INSERT INTO Players (Id, Initials, Name, [Default]) VALUES (1, 'AAA', 'Default', '1');

-- SYSTEM SETTINGS
INSERT INTO Settings (Id, Value, Type, Parent, Options, Info) VALUES
('BALLS_PER_GAME', '3', 4, 'GENERAL', '[1,2,3,4,5]', 'Number of balls per game 1-5'),
('BALL_SAVE_TIME', '8', 4, 'GENERAL', '[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25]', 'Ball saver time'),
('BALL_SEARCH_TIME', '10', 4, 'GENERAL', '[8-30]', 'Timeout to search for balls and pulse coils'),
('IDLE_SHOOTER_TIMEOUT', '60', 4, 'GENERAL', '[0,30,60,90,120,150]', 'Auto launch ball if idle in plunger lane, 0 disabled'),
('ALLOW_RESTART', '1', 4, 'GENERAL', '[0, 1]', 'Allow game restart from holding start.'),
('MATCH_PERCENT', '5', 4, 'GENERAL', '[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20]', 'Match percent, 0 off'),
('XB_RESERVE', '0', 4, 'GENERAL', '[0, 1]', 'If enabled and in multiplayer the rounds continue with xb given at the end'),
-- todo: chase ball for ball search
('XB_GAME_MAX', '-1', 4, 'EXTRA BALLS', '[-1,0,1,2,3,4,5,6]', 'Max extra balls, -1 unlimited'),
('XB_PLAYER_MAX', '3', 4, 'EXTRA BALLS', '[-1,0,1,2,3,4,5,6]', 'Max extra balls PLAYER, -1 unlimited'),
('TILT_WARNINGS', '2', 4, 'TILT', '[0,1,2,3,4,5,6,7,8,9,10]', 'Number of tilt warnings'),
('MASTER', '-6', 4, 'AUDIO', '[0,-1,-2,-3,-4,-5,-6,-7,-8,-9,-10,-11,-12,-13,-14,-15,-16,-17,-18,-19,-20,-21,-22,-23,-24,-25,-26,-27,-28,-29,-30]', 'Master volume'),
('MUSIC', '-6', 4, 'AUDIO', '[0,-1,-2,-3,-4,-5,-6,-7,-8,-9,-10,-11,-12,-13,-14,-15,-16,-17,-18,-19,-20,-21,-22,-23,-24,-25,-26,-27,-28,-29,-30]', 'Music Volume'),
('VOICE', '-6', 4, 'AUDIO', '[0,-1,-2,-3,-4,-5,-6,-7,-8,-9,-10,-11,-12,-13,-14,-15,-16,-17,-18,-19,-20,-21,-22,-23,-24,-25,-26,-27,-28,-29,-30]', 'Voice volume'),
('FX', '-6', 4, 'AUDIO', '[0,-1,-2,-3,-4,-5,-6,-7,-8,-9,-10,-11,-12,-13,-14,-15,-16,-17,-18,-19,-20,-21,-22,-23,-24,-25,-26,-27,-28,-29,-30]', 'FX volume'),
('ATTRACT_MUSIC', '1', 4, 'AUDIO', '[0, 1]', 'Allow music to play in attract');
--REPLAY
--SCORES
-- PRICING SETTINGS
-- GAME SETTINGS
-- COIL SETTINGS - flipper strength etc - use the actual pulse time rather than create new tables, they will be 1-255 anyway
-- LOG SETTINGS
--INSERT INTO Settings (Id, Value, Type) VALUES ('LOG_LEVEL_GAME', NULL, 'GAME');
--INSERT INTO Settings (Id, Value, Type) VALUES ('LOG_LEVEL_DISPLAY', '0', 'DISPLAY'); -- TraceLogType 0 == ALL

---- DEVELOPER SETTINGS
--INSERT INTO Settings (Id, Value, Type) VALUES ('PLAYBACK_ENABLED', '0', 'PLAYBACK');
--INSERT INTO Settings (Id, Value, Type) VALUES ('PLAYBACK_RECORDING_ID', NULL, 'PLAYBACK');
--INSERT INTO Settings (Id, Value, Type) VALUES ('RECORDING_ENABLED', '0', 'RECORDING');
--INSERT INTO Settings (Id, Value, Type) VALUES ('RECORDING_SET_PLAYBACK_ON_END', NULL, 'RECORDING');

-- AUDITS (STANDARD)
INSERT INTO Audits (Id, Value, Type, Info) VALUES
('GAMES_STARTED', '0', 0, 'Games started log'), ('GAMES_PLAYED', '0', 0, 'Games completed log'), ('XB_AWARDED', '0', 0, 'Total extra balls awarded'),
('REPLAYS', '0', 0, 'Total replays awarded'), ('MATCHES', '0', 0, 'Total Matches Awarded'),
('POWERED_ON_TIMES', '0', 0, 'Times machine powered on'), ('TOTAL_BALLS_PLAYED', '0', 0, 'Total balls played');

-- AUDITS (GAME) TODO: LOG SWITCHES, MODES, TIMES


COMMIT TRANSACTION;