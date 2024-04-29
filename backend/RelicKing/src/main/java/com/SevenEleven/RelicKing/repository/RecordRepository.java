package com.SevenEleven.RelicKing.repository;

import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;

public interface RecordRepository extends JpaRepository<Record, Integer> {

	Optional<Record> findByMemberAndStage(Member member, int stage);
}
