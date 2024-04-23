package com.SevenEleven.RelicKing.repository;

import org.springframework.data.jpa.repository.JpaRepository;

import com.SevenEleven.RelicKing.entity.Record;

public interface RecordRepository extends JpaRepository<Record, Integer> {
}
