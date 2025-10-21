-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Oct 21, 2025 at 05:31 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `gamedb`
--

-- --------------------------------------------------------

--
-- Table structure for table `player_info`
--

CREATE TABLE `player_info` (
  `player_id` int(11) NOT NULL,
  `server_id` int(11) DEFAULT NULL,
  `posX` float DEFAULT NULL,
  `posY` float DEFAULT NULL,
  `posZ` float DEFAULT NULL,
  `rotX` float DEFAULT NULL,
  `rotY` float DEFAULT NULL,
  `rotZ` float DEFAULT NULL,
  `last_heartbeat` text DEFAULT NULL,
  `nick` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `servers`
--

CREATE TABLE `servers` (
  `id` int(11) NOT NULL,
  `name` text NOT NULL,
  `code` text NOT NULL,
  `creation_time` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `player_info`
--
ALTER TABLE `player_info`
  ADD PRIMARY KEY (`player_id`),
  ADD KEY `fk_session` (`server_id`);

--
-- Indexes for table `servers`
--
ALTER TABLE `servers`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id` (`id`),
  ADD UNIQUE KEY `code` (`code`) USING HASH;

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `player_info`
--
ALTER TABLE `player_info`
  MODIFY `player_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `servers`
--
ALTER TABLE `servers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `player_info`
--
ALTER TABLE `player_info`
  ADD CONSTRAINT `fk_session` FOREIGN KEY (`server_id`) REFERENCES `servers` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
