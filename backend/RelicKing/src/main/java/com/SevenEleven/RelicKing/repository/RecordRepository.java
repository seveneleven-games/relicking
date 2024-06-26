package com.SevenEleven.RelicKing.repository;

import java.util.List;
import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;

public interface RecordRepository extends JpaRepository<Record, Integer> {

	Optional<Record> findByMemberAndStage(Member member, int stage);

	List<Record> findTop100ByStageOrderByDifficultyDescUpdatedDate(int stage);

	@Query(value = "SELECT ranking.stage_rank FROM (" +
			"SELECT r.member_id as memberId, RANK() OVER (ORDER BY r.difficulty DESC, r.updated_date) as stage_rank " +
			"FROM record r WHERE r.stage = :stage) ranking " +
			"WHERE ranking.memberId = :memberId", nativeQuery = true)
	int findRankByMemberAndStage(@Param("memberId") int memberId, @Param("stage") int stage);
}
