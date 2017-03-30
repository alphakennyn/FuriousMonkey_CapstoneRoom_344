-- phpMyAdmin SQL Dump
-- version 4.6.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Mar 30, 2017 at 03:08 PM
-- Server version: 5.7.14
-- PHP Version: 5.6.25

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `reservation_system`
--

-- --------------------------------------------------------

--
-- Table structure for table `equipment`
--

CREATE TABLE `equipment` (
  `equipmentID` int(3) NOT NULL,
  `equipmentName` varchar(30) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `equipment`
--

INSERT INTO `equipment` (`equipmentID`, `equipmentName`) VALUES
(1, 'Projector'),
(2, 'Marker'),
(3, 'Projector'),
(4, 'Computer'),
(5, 'Computer');

-- --------------------------------------------------------

--
-- Table structure for table `equipmentwaitsfor`
--

CREATE TABLE `equipmentwaitsfor` (
  `timeSlotID` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `dateTime` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `reservation`
--

CREATE TABLE `reservation` (
  `reservationID` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `roomID` int(11) NOT NULL,
  `description` text NOT NULL,
  `date` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `room`
--

CREATE TABLE `room` (
  `roomID` int(11) NOT NULL,
  `roomNum` varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `room`
--

INSERT INTO `room` (`roomID`, `roomNum`) VALUES
(1, 'H961-1'),
(2, 'H961-2'),
(3, 'H961-3'),
(4, 'H961-4'),
(5, 'H961-5');

-- --------------------------------------------------------

--
-- Table structure for table `timeslot`
--

CREATE TABLE `timeslot` (
  `timeSlotID` int(11) NOT NULL,
  `reservationID` int(11) NOT NULL,
  `hour` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `userID` int(11) NOT NULL,
  `username` varchar(30) NOT NULL,
  `password` varchar(30) NOT NULL,
  `name` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`userID`, `username`, `password`, `name`) VALUES
(1, 'h_ortiz', 'hortiz', 'Hannah Ortiz'),
(2, 'i_koun', 'ikoun', 'Ideawin Koun'),
(3, 'p_lim', 'plim', 'Philip Lim'),
(4, 'admin', 'admin', 'Administrator');

-- --------------------------------------------------------

--
-- Table structure for table `waitsfor`
--

CREATE TABLE `waitsfor` (
  `timeSlotID` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `dateTime` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `equipment`
--
ALTER TABLE `equipment`
  ADD PRIMARY KEY (`equipmentID`);

--
-- Indexes for table `equipmentwaitsfor`
--
ALTER TABLE `equipmentwaitsfor`
  ADD PRIMARY KEY (`timeSlotID`,`userID`);

--
-- Indexes for table `reservation`
--
ALTER TABLE `reservation`
  ADD PRIMARY KEY (`reservationID`),
  ADD KEY `userID` (`userID`),
  ADD KEY `roomID` (`roomID`);

--
-- Indexes for table `room`
--
ALTER TABLE `room`
  ADD PRIMARY KEY (`roomID`);

--
-- Indexes for table `timeslot`
--
ALTER TABLE `timeslot`
  ADD PRIMARY KEY (`timeSlotID`),
  ADD KEY `reservationID` (`reservationID`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`userID`),
  ADD UNIQUE KEY `username` (`username`);

--
-- Indexes for table `waitsfor`
--
ALTER TABLE `waitsfor`
  ADD PRIMARY KEY (`timeSlotID`,`userID`),
  ADD KEY `timeSlotID` (`timeSlotID`),
  ADD KEY `waitsfor_ibfk_1` (`userID`);

--
-- Constraints for dumped tables
--

--
-- Constraints for table `reservation`
--
ALTER TABLE `reservation`
  ADD CONSTRAINT `reservation_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `user` (`userID`),
  ADD CONSTRAINT `reservation_ibfk_2` FOREIGN KEY (`roomID`) REFERENCES `room` (`roomID`);

--
-- Constraints for table `timeslot`
--
ALTER TABLE `timeslot`
  ADD CONSTRAINT `timeslot_ibfk_1` FOREIGN KEY (`reservationID`) REFERENCES `reservation` (`reservationID`) ON DELETE CASCADE;

--
-- Constraints for table `waitsfor`
--
ALTER TABLE `waitsfor`
  ADD CONSTRAINT `waitsfor_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `user` (`userID`),
  ADD CONSTRAINT `waitsfor_ibfk_2` FOREIGN KEY (`timeSlotID`) REFERENCES `timeslot` (`timeSlotID`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
